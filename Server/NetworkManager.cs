namespace Server
{
    using System;
    using System.Net;

    using Common.Network;

    using Database;

    public class NetworkManager
    {
        #region Constants

        private const int WS_PORT = 65000;
        private const int TCP_PORT = 65001;

        #endregion Constants

        #region Fields

        private readonly WsServer _wsServer;

        private TextMessageService _txtMsgService = new TextMessageService();
        private ClientEventService _clientEventService = new ClientEventService();

        #endregion Fields

        #region Constructors

        public NetworkManager()
        {
            _wsServer = new WsServer(new IPEndPoint(IPAddress.Any, WS_PORT));
            _wsServer.ConnectionStateChanged += HandleConnectionStateChanged;
            _wsServer.ConnectionReceived += HandleConnectionReceived;
            _wsServer.MessageReceived += HandleMessageReceived;
            _wsServer.ErrorReceived += HandleErrorReceived;
            _wsServer.FilterReceived += HandleFilterReceived;
        }

        #endregion Constructors

        #region Methods

        public void Start()
        {
            Console.WriteLine($"WebSocketServer: {IPAddress.Any}:{WS_PORT}");
            _wsServer.Start();
        }

        public void Stop()
        {
            _wsServer.Stop();
        }

        private void HandleMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.Target == null)
            {
                Console.WriteLine($"{e.Source} : {e.Message}");
            }
            else
            {
                Console.WriteLine($"{e.Source} to {e.Target} : {e.Message}");
            }

            _txtMsgService.AddMessage(e.Source, e.Target, e.Message, e.Date);

            _wsServer.SendMessageBroadcast(e.Source, e.Target, e.Message);
        }

        private void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            string clientState = e.IsConnected ? "подключен" : "отключен";
            string message = $"{e.Client} {clientState}.";

            if (e.IsConnected)
            {
                var chatHistory = _txtMsgService.GetClientMessages(e.Client);
                _wsServer.SendChatHistory(e.Client, chatHistory);
            }

            _clientEventService.AddClientEvent(MessageType.Event, message, e.Date);

            Console.WriteLine($"{message}");
            
            _wsServer.SendMessageBroadcast(e.Client, null, clientState);
        }

        private void HandleConnectionReceived(object sender, ConnectionReceivedEventArgs e)
        {
            _wsServer.SendConnectionBroadcast(e.Login, e.IsConnected);
        }

        private void HandleErrorReceived(object sender, ErrorReceivedEventArgs e)
        {
            _clientEventService.AddClientEvent(MessageType.Error, e.Reason, e.Date);
        }

        private void HandleFilterReceived(object sender, FilterReceivedEventArgs e)
        {

        }

        #endregion Methods
        }
}
