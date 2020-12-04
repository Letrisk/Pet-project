namespace Common.Network
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.ObjectModel;
    using System.Threading;

    using WebSocketSharp;

    using Messages;
    using Network;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class ChatController : IChatController
    {
        #region Fields

        private readonly ConcurrentQueue<MessageContainer> _sendQueue;

        //private ObservableCollection<string> _clientsList;

        private WebSocket _socket;

        private int _sending;
        private string _login;

        #endregion Fields

        #region Properties

        public bool IsConnected => _socket?.ReadyState == WebSocketState.Open;

        public WebSocket Socket
        {
            get => _socket;
            set
            {
                _socket = value;
                _socket.OnOpen += OnOpen;
                _socket.OnMessage += OnMessage;
                _socket.OnClose += OnClose;
            }
        }

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
            }
        }

        #endregion Properties

        #region Events

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
        public event EventHandler<ConnectionReceivedEventArgs> ConnectionReceived;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ChatHistoryReceivedEventArgs> ChatHistoryReceived;
        public event EventHandler<FilteredMessagesReceivedEventArgs> FilteredMessagesReceived;
        public event EventHandler<ClientsListReceivedEventArgs> ClientsListReceived;

        #endregion Events

        #region Constructors

        public ChatController()
        {
            _sendQueue = new ConcurrentQueue<MessageContainer>();
            _sending = 0;
        }

        #endregion Constructors

        #region Methods

        public void Disconnect()
        {
            if (_socket == null)
                return;

            if (IsConnected)
                _socket.CloseAsync();

            _socket.OnOpen -= OnOpen;
            _socket.OnClose -= OnClose;
            _socket.OnMessage -= OnMessage;

            _socket = null;
            _login = string.Empty;
        }

        public void Send(string source, string target, string message)
        {
            _sendQueue.Enqueue(new MessageRequest(source, target, message).GetContainer());

            if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
                SendImpl();
        }

        private void SendCompleted(bool completed)
        {
            if (!completed)
            {
                Disconnect();
                ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(_login, DateTime.Now, false));
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
                        _login = string.Empty;
                        string source = string.Empty;
                        MessageReceived?.Invoke(this, new MessageReceivedEventArgs(source, _login, connectionResponse.Reason, connectionResponse.Date));
                    }
                    ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(_login, DateTime.Now, true, connectionResponse.OnlineClients));
                    break;
                case nameof(ConnectionBroadcast):
                    var connectionBroadcast = ((JObject)container.Payload).ToObject(typeof(ConnectionBroadcast)) as ConnectionBroadcast;
                    ConnectionReceived?.Invoke(this, new ConnectionReceivedEventArgs(connectionBroadcast.Login, connectionBroadcast.IsConnected, connectionBroadcast.Date));
                    break;
                case nameof(MessageBroadcast):
                    var messageBroadcast = ((JObject)container.Payload).ToObject(typeof(MessageBroadcast)) as MessageBroadcast;
                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs(messageBroadcast.Source, messageBroadcast.Target, messageBroadcast.Message, messageBroadcast.Date));
                    break;
                case nameof(ChatHistoryResponse):
                    var chatHistoryResponse = ((JObject)container.Payload).ToObject(typeof(ChatHistoryResponse)) as ChatHistoryResponse;
                    ChatHistoryReceived?.Invoke(this, new ChatHistoryReceivedEventArgs(chatHistoryResponse.ClientMessages));
                    break;
                case nameof(FilterResponse):
                    var filterResponse = ((JObject)container.Payload).ToObject(typeof(FilterResponse)) as FilterResponse;
                    FilteredMessagesReceived?.Invoke(this, new FilteredMessagesReceivedEventArgs(filterResponse.FilteredMessages));
                    break;
                case nameof(ClientsListResponse):
                    var clientsListResponse = ((JObject)container.Payload).ToObject(typeof(ClientsListResponse)) as ClientsListResponse;
                    ClientsListReceived?.Invoke(this, new ClientsListReceivedEventArgs(clientsListResponse.Clients));
                    break;
            }
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(_login, DateTime.Now, false));
        }

        private void OnOpen(object sender, System.EventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(_login, DateTime.Now, true));
        }

        #endregion Methods
    }
}
