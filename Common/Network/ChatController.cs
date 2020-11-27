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
        private bool _isEnable;

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

        /*public ObservableCollection<string> ClientsList
        {
            get => _clientsList;
            set
            {
                _clientsList = value;
            }
        }*/

        public ConcurrentQueue<MessageContainer> SendQueue
        {
            get => _sendQueue;
        }

        public int Sending
        {
            get => _sending;
            set
            {
                _sending = value;
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

        public bool IsEnable
        {
            get => _isEnable;
            set
            {
                _isEnable = value;
            }
        }

        #endregion Properties

        #region Events

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
        public event EventHandler<ConnectionReceivedEventArgs> ConnectionReceived;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

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
