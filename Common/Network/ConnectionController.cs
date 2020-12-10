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

        private readonly IChatController _chatController;
        private readonly IEventLogController _eventLogController;
        private readonly IGroupChatController _groupChatController;
        private readonly IController _controller;

        private WebSocket _socket;

        private string _login;

        #endregion Fields

        #region Properties

        public bool IsConnected => _socket?.ReadyState == WebSocketState.Open;

        #endregion Properties

        #region Events

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
        public event EventHandler<ErrorReceivedEventArgs> ErrorReceived;

        #endregion Events

        #region Constructors

        public ConnectionController(IController controller, IChatController chatController, IEventLogController eventLogController,
                                    IGroupChatController groupChatController)
        {
            _controller = controller;
            _chatController = chatController;
            _eventLogController = eventLogController;
            _groupChatController = groupChatController;
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

            _controller.Socket = _socket;
            _chatController.Socket = _socket;
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
            _groupChatController.Login = login;
            _controller.Send(new ConnectionRequest(_login).GetContainer());
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
                    }

                    _chatController.Login = _login;
                    _eventLogController.Login = _login;

                    ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(_login, DateTime.Now, true, connectionResponse.OnlineClients));
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
