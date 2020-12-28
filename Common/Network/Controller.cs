namespace Common.Network
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    using WebSocketSharp;

    using Messages;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class Controller : IController
    {
        #region Fields

        private readonly ConcurrentQueue<MessageContainer> _sendQueue;

        private WebSocket _socket;

        private int _sending;

        #endregion Fields

        #region Properties

        public string Login { get; set; }

        public bool IsConnected => _socket?.ReadyState == WebSocketState.Open;

        #endregion Properties

        #region Events

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
        public event EventHandler<ErrorReceivedEventArgs> ErrorReceived;
        public event EventHandler<ConnectionReceivedEventArgs> ConnectionReceived;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ChatHistoryReceivedEventArgs> ChatHistoryReceived;
        public event EventHandler<FilteredMessagesReceivedEventArgs> FilteredMessagesReceived;
        public event EventHandler<ClientsListReceivedEventArgs> ClientsListReceived;
        public event EventHandler<GroupsReceivedEventArgs> GroupsReceived;

        #endregion Events

        #region Constructors

        public Controller()
        {
            _sendQueue = new ConcurrentQueue<MessageContainer>();
            _sending = 0;
        }

        #endregion Constructors

        #region Methods

        public void Connect(string address, string port)
        {
            _socket = new WebSocket($"ws://{address}:{port}");
            _socket.OnOpen += OnOpen;
            _socket.OnClose += OnClose;
            _socket.OnMessage += OnMessage;
            _socket.ConnectAsync();
        }

        public void Send(MessageContainer messageContainer)
        {
            _sendQueue.Enqueue(messageContainer);

            if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
                SendImpl();
        }

        public void Disconnect()
        {
            if (_socket == null)
                return;

            if (IsConnected)
                _socket.CloseAsync();

            _socket.OnOpen -= OnOpen;
            _socket.OnClose -= OnClose;
            _socket.OnMessage -= OnMessage;

            Login = String.Empty;
            _socket = null;
        }

        private void SendCompleted(bool completed)
        {
            if (!completed)
            {
                Disconnect();
                ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(Login, DateTime.Now, false));
                return;
            }

            SendImpl();
        }

        private void SendImpl()
        {
            if (!IsConnected)
                return;

            if (!_sendQueue.TryDequeue(out var message) && Interlocked.CompareExchange(ref _sending, 0, 1) == 1)
                return;

            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            string serializedMessages = JsonConvert.SerializeObject(message, settings);
            _socket.SendAsync(serializedMessages, SendCompleted);
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            if (!e.IsText)
                return;

            var container = JsonConvert.DeserializeObject<MessageContainer>(e.Data);

            switch (container.Identifier)
            {
                case nameof(ConnectionResponse):
                    var connectionResponse = ((JObject)container.Payload).ToObject(typeof(ConnectionResponse)) as ConnectionResponse;
                    if (connectionResponse.Result == ResultCodes.Failure)
                    {
                        Login = string.Empty;
                        string source = string.Empty;
                        ErrorReceived?.Invoke(this, new ErrorReceivedEventArgs(connectionResponse.Reason, connectionResponse.Date));
                    }
                    if (!String.IsNullOrEmpty(Login))
                    {
                        ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(Login, DateTime.Now, true, connectionResponse.OnlineClients));
                    }
                    break;
                case nameof(ConnectionBroadcast):
                    var connectionBroadcast = ((JObject)container.Payload).ToObject(typeof(ConnectionBroadcast)) as ConnectionBroadcast;
                    if (connectionBroadcast.Login != Login)
                    {
                        ConnectionReceived?.Invoke(this, new ConnectionReceivedEventArgs(connectionBroadcast.Login, connectionBroadcast.IsConnected, connectionBroadcast.Date));
                    }
                    break;
                case nameof(MessageBroadcast):
                    var messageBroadcast = ((JObject)container.Payload).ToObject(typeof(MessageBroadcast)) as MessageBroadcast;
                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs(messageBroadcast.Source, messageBroadcast.Target, messageBroadcast.Message, messageBroadcast.Date,
                                                                               messageBroadcast.GroupName));
                    break;
                case nameof(ClientsListResponse):
                    var clientsListResponse = ((JObject)container.Payload).ToObject(typeof(ClientsListResponse)) as ClientsListResponse;
                    ClientsListReceived?.Invoke(this, new ClientsListReceivedEventArgs(clientsListResponse.Clients));
                    break;
                case nameof(ChatHistoryResponse):
                    var chatHistoryResponse = ((JObject)container.Payload).ToObject(typeof(ChatHistoryResponse)) as ChatHistoryResponse;
                    ChatHistoryReceived?.Invoke(this, new ChatHistoryReceivedEventArgs(chatHistoryResponse.ClientMessages));
                    break;
                case nameof(FilterResponse):
                    var filterResponse = ((JObject)container.Payload).ToObject(typeof(FilterResponse)) as FilterResponse;
                    FilteredMessagesReceived?.Invoke(this, new FilteredMessagesReceivedEventArgs(filterResponse.FilteredMessages));
                    break;
                case nameof(GroupsListResponse):
                    var groupsListResponse = ((JObject)container.Payload).ToObject(typeof(GroupsListResponse)) as GroupsListResponse;
                    GroupsReceived?.Invoke(this, new GroupsReceivedEventArgs(groupsListResponse.Groups));
                    break;
                case nameof(GroupBroadcast):
                    var groupBroadcast = ((JObject)container.Payload).ToObject(typeof(GroupBroadcast)) as GroupBroadcast;
                    GroupsReceived?.Invoke(this, new GroupsReceivedEventArgs(groupBroadcast.Group));
                    break;
            }
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(Login, DateTime.Now, false));
        }

        private void OnOpen(object sender, System.EventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(Login, DateTime.Now, true));
        }

        #endregion Methods
    }
}
