namespace Common.Network
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net;

    using Messages;
    using _EventArgs_;
    using _Enums_;

    using Newtonsoft.Json.Linq;

    using WebSocketSharp.Server;

    public class WsServer
    {
        #region Fields

        private readonly IPEndPoint _listenAddress;
        private readonly ConcurrentDictionary<Guid, WsConnection> _connections;

        private WebSocketServer _server;

        #endregion Fields

        #region Events

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
        public event EventHandler<ConnectionReceivedEventArgs> ConnectionReceived;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion Events

        #region Constructors

        public WsServer(IPEndPoint listenAddress)
        {
            _listenAddress = listenAddress;
            _connections = new ConcurrentDictionary<Guid, WsConnection>();
        }

        #endregion Constructors

        #region Methods

        public void Start()
        {
            _server = new WebSocketServer(_listenAddress.Address, _listenAddress.Port, false);

            _server.AddWebSocketService<WsConnection>("/",
                client =>
                {
                    client.AddServer(this);
                });
            _server.Start();
        }

        public void Stop()
        {
            _server?.Stop();
            _server = null;

            var connections = _connections.Select(item => item.Value).ToArray();
            foreach (var connection in connections)
            {
                connection.Close();
            }

            _connections.Clear();
        }

        public void SendMessageBroadcast(string source, string target, string message)
        {
            var messageBroadcast = new MessageBroadcast(source, target, message, DateTime.Now).GetContainer();

            if (target == null)
            {
                foreach (var connection in _connections)
                {
                    connection.Value.Send(messageBroadcast);
                }
            }
            else
            {
                foreach (var connection in _connections)
                {
                    if (connection.Value.Login == target)
                    {
                        connection.Value.Send(messageBroadcast);
                    }
                }
            }
        }

        public void SendConnectionBroadcast (string login, bool isConnected)
        {
            var connectionBroadcast = new ConnectionBroadcast(login, isConnected, DateTime.Now).GetContainer();

            foreach (var connection in _connections)
            {
                connection.Value.Send(connectionBroadcast);
            }
        }


        internal void HandleMessage(Guid clientId, MessageContainer container)
        {
            if (!_connections.TryGetValue(clientId, out WsConnection connection))
                return;

            switch (container.Identifier)
            {
                case nameof(ConnectionRequest):
                    var connectionRequest = ((JObject)container.Payload).ToObject(typeof(ConnectionRequest)) as ConnectionRequest;
                    var connectionResponse = new ConnectionResponse { Result = ResultCodes.Ok,
                                                                      IsSuccessful = true,
                                                                    };
                    if (_connections.Values.Any(item => item.Login == connectionRequest.Login))
                    {
                        connectionResponse.Result = ResultCodes.Failure;
                        connectionResponse.IsSuccessful = false;
                        connectionResponse.Reason = $"Клиент с именем '{connectionRequest.Login}' уже подключен.";
                        connection.Send(connectionResponse.GetContainer());
                    }
                    else
                    {
                        connection.Login = connectionRequest.Login;
                        connectionResponse.OnlineClients = _connections.Select(item => item.Value.Login).ToArray();
                        connection.Send(connectionResponse.GetContainer());
                        ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(connection.Login, DateTime.Now, true));
                        ConnectionReceived?.Invoke(this, new ConnectionReceivedEventArgs(connection.Login, true, DateTime.Now));
                    }
                    break;
                case nameof(MessageRequest):
                    var messageRequest = ((JObject)container.Payload).ToObject(typeof(MessageRequest)) as MessageRequest;
                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs(messageRequest.Source, messageRequest.Target, messageRequest.Message, DateTime.Now));
                    break;
            }
        }

        internal void AddConnection(WsConnection connection)
        {
            _connections.TryAdd(connection.Id, connection);
        }

        internal void FreeConnection(Guid connectionId)
        {
            if (_connections.TryRemove(connectionId, out WsConnection connection) && !string.IsNullOrEmpty(connection.Login))
            {
                ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(connection.Login, DateTime.Now, false));
                ConnectionReceived?.Invoke(this, new ConnectionReceivedEventArgs(connection.Login, false, DateTime.Now));
            }
        }

        #endregion Methods
    }
}
