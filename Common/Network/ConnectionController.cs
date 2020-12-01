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
    public class ConnectionController : IConnectionController
    {
        #region Fields

        private readonly ConcurrentQueue<MessageContainer> _sendQueue;

        private readonly IChatController _chatController;
        private readonly IEventLogController _eventLogController;

        private WebSocket _socket;

        private int _sending;
        private string _login;

        #endregion Fields

        #region Properties

        public bool IsConnected => _socket?.ReadyState == WebSocketState.Open;

        #endregion Properties

        #region Events

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
        public event EventHandler<ErrorReceivedEventArgs> ErrorReceived;
        /*public event EventHandler<ConnectionReceivedEventArgs> ConnectionReceived;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ChatHistoryReceivedEventArgs> ChatHistoryReceived;*/

        #endregion Events

        #region Constructors

        public ConnectionController(IChatController chatController, IEventLogController eventLogController)
        {
            _chatController = chatController;
            _eventLogController = eventLogController;

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

            _chatController.Socket = _socket;
            _eventLogController.Socket = _socket;
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

            _socket = null;
            _login = string.Empty;
        }

        public void Login(string login)
        {
            _login = login;
            _sendQueue.Enqueue(new ConnectionRequest(_login).GetContainer());

            if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
                SendImpl();
        }

        private void SendCompleted(bool completed)
        {
            // При отправке произошла ошибка.
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
                        ErrorReceived?.Invoke(this, new ErrorReceivedEventArgs(connectionResponse.Reason, connectionResponse.Date));
                        //MessageReceived?.Invoke(this, new MessageReceivedEventArgs(source, _login, connectionResponse.Reason, connectionResponse.Date));
                    }

                    //_chatController.Sending = _sending;
                    _chatController.Login = _login;
                    //_chatController.IsEnable = true;
                    /*_chatController.ConnectionStateChanged += ConnectionStateChanged;
                    _chatController.ConnectionReceived += ConnectionReceived;
                    _chatController.MessageReceived += MessageReceived;*/

                    ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(_login, DateTime.Now, true, connectionResponse.OnlineClients));
                    break;
                /*case nameof(ConnectionBroadcast):
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
                    break;*/
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
