namespace Common.Network
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net;
    using System.Timers;

    using Messages;

    using Newtonsoft.Json.Linq;

    using WebSocketSharp.Server;

    public class WsServer
    {
        #region Fields

        private readonly IPEndPoint _listenAddress;
        private readonly ConcurrentDictionary<Guid, WsConnection> _connections;

        private readonly Dictionary<Guid, long> _timeoutClients;
        private readonly Timer _timeoutTimer;
        private long _timeout;

        private WebSocketServer _server;

        #endregion Fields

        #region Events

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
        public event EventHandler<ConnectionReceivedEventArgs> ConnectionReceived;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ErrorReceivedEventArgs> ErrorReceived;
        public event EventHandler<FilterReceivedEventArgs> FilterReceived;
        public event EventHandler<CreateGroupReceivedEventArgs> CreateGroupReceived;
        public event EventHandler<LeaveGroupReceivedEventArgs> LeaveGroupReceived;

        #endregion Events

        #region Constructors

        public WsServer(IPEndPoint listenAddress)
        {
            _listenAddress = listenAddress;
            _connections = new ConcurrentDictionary<Guid, WsConnection>();
            _timeoutClients = new Dictionary<Guid, long>();
            _timeoutTimer = new Timer();

            _timeoutTimer.AutoReset = true;
            _timeoutTimer.Interval = 10000;
            _timeoutTimer.Elapsed += OnTimeoutEvent;
            _timeoutTimer.Enabled = true;
            _timeoutTimer.Start();
        }

        #endregion Constructors

        #region Methods

        public long Timeout
        {
            get => _timeout;
            set => _timeout = value;
        }

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

            _timeoutClients.Clear();
            _connections.Clear();
        }

        public void Send(string source, string target, MessageContainer message)
        {
            if (String.IsNullOrEmpty(target))
            {
                foreach (var connection in _connections)
                {
                    connection.Value?.Send(message);
                }
            }
            else
            {
                foreach (var connection in _connections)
                {
                    if (connection.Value.Login == target || connection.Value.Login == source)
                    {
                        connection.Value?.Send(message);
                    }
                }
            }
        }

        private void OnTimeoutEvent(object sender, ElapsedEventArgs e)
        {
            var timedClients = _timeoutClients.Where(item => DateTime.Now.Ticks - item.Value >= Timeout).Select(item => item.Key).ToList();
            foreach (Guid client in timedClients)
            {
                _timeoutClients?.Remove(client);
                _connections[client]?.Close();
            }
        }


        internal void HandleMessage(Guid clientId, MessageContainer container)
        {
            if (!_connections.TryGetValue(clientId, out WsConnection connection))
                return;

            _timeoutClients[clientId] = DateTime.Now.Ticks;

            switch (container.Identifier)
            {
                case nameof(ConnectionRequest):
                    var connectionRequest = ((JObject)container.Payload).ToObject(typeof(ConnectionRequest)) as ConnectionRequest;
                    var connectionResponse = new ConnectionResponse { Result = ResultCodes.Ok,
                                                                      IsSuccessful = true,
                                                                    };
                    if (_connections.Values.Any(item => item.Login == connectionRequest.Login))
                    {
                        string reason = $"Клиент с именем '{connectionRequest.Login}' уже подключен.";
                        connectionResponse.Result = ResultCodes.Failure;
                        connectionResponse.IsSuccessful = false;
                        connectionResponse.Reason = reason;
                        connection.Send(connectionResponse.GetContainer());
                        ErrorReceived?.Invoke(this, new ErrorReceivedEventArgs(reason, DateTime.Now));
                    }
                    else
                    {
                        connection.Login = connectionRequest.Login;
                        connectionResponse.OnlineClients = _connections.Where(item => item.Value.Login != null).Select(item => item.Value.Login).ToArray();
                        connection.Send(connectionResponse.GetContainer());
                        ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(connection.Login, DateTime.Now, true));
                        ConnectionReceived?.Invoke(this, new ConnectionReceivedEventArgs(connection.Login, true, DateTime.Now));
                    }
                    break;
                case nameof(MessageRequest):
                    var messageRequest = ((JObject)container.Payload).ToObject(typeof(MessageRequest)) as MessageRequest;
                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs(connection.Login, messageRequest.Target, messageRequest.Message, DateTime.Now, messageRequest.GroupName));
                    break;
                case nameof(FilterRequest):
                    var filterRequest = ((JObject)container.Payload).ToObject(typeof(FilterRequest)) as FilterRequest;
                    FilterReceived?.Invoke(this, new FilterReceivedEventArgs(connection.Login, filterRequest.FirstDate, filterRequest.SecondDate, filterRequest.MessageTypes));
                    break;
                case nameof(CreateGroupRequest):
                    var createGroupRequest = ((JObject)container.Payload).ToObject(typeof(CreateGroupRequest)) as CreateGroupRequest;
                    createGroupRequest.Clients.Add(connection.Login);
                    CreateGroupReceived?.Invoke(this, new CreateGroupReceivedEventArgs(createGroupRequest.GroupName, createGroupRequest.Clients));
                    break;
                case nameof(LeaveGroupRequest):
                    var leaveGroupRequest = ((JObject)container.Payload).ToObject(typeof(LeaveGroupRequest)) as LeaveGroupRequest;
                    LeaveGroupReceived?.Invoke(this, new LeaveGroupReceivedEventArgs(connection.Login, leaveGroupRequest.GroupName));
                    break;
            }
        }

        internal void AddConnection(WsConnection connection)
        {
            _connections.TryAdd(connection.Id, connection);
            _timeoutClients.Add(connection.Id, DateTime.Now.Ticks);
        }

        internal void FreeConnection(Guid id)
        {
            if (_connections.TryRemove(id, out WsConnection connection) && !string.IsNullOrEmpty(connection.Login))
            {
                
                ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(connection.Login, DateTime.Now, false));
                ConnectionReceived?.Invoke(this, new ConnectionReceivedEventArgs(connection.Login, false, DateTime.Now));
            }

            _timeoutClients.Remove(id);
        }

        #endregion Methods
    }
}
