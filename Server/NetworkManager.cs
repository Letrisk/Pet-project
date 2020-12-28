namespace Server
{
    using System;
    using System.Net;
    using System.Collections.Generic;
    using System.Configuration;

    using Common.Network;
    using Common.Network.Messages;

    using Database;

    public class NetworkManager
    {
        #region Fields

        private readonly string _transport;
        private readonly long _timeout;
        private readonly int _port;
        private readonly IPAddress _ip;
        private readonly ConnectionStringSettings _connectionString;

        private readonly WsServer _wsServer;

        private TextMessageService _txtMsgService;
        private ClientEventService _clientEventService;
        private ClientService _clientService;
        private GroupService _groupService;

        #endregion Fields

        #region Constructors

        public NetworkManager()
        {
            SettingsManager settingsManager = new SettingsManager("ServerConfig.xml");
            _transport = settingsManager.Transport;
            _ip = settingsManager.Ip;
            _port = settingsManager.Port;
            _timeout = settingsManager.Timeout;
            _connectionString = settingsManager.ConnectionSettings;

            DatabaseController databaseController = new DatabaseController(_connectionString);

            if (_transport == "WebSocket")
            {
                _wsServer = new WsServer(new IPEndPoint(_ip, _port));
                _wsServer.ConnectionStateChanged += HandleConnectionStateChanged;
                _wsServer.ConnectionReceived += HandleConnectionReceived;
                _wsServer.MessageReceived += HandleMessageReceived;
                _wsServer.ErrorReceived += HandleErrorReceived;
                _wsServer.FilterReceived += HandleFilterReceived;
                _wsServer.CreateGroupReceived += HandleCreateGroupReceived;
                _wsServer.LeaveGroupReceived += HandleLeaveGroupReceived;
                _wsServer.Timeout = _timeout;
            }

            _txtMsgService = new TextMessageService(databaseController);
            _clientEventService = new ClientEventService(databaseController);
            _clientService = new ClientService(databaseController);
            _groupService = new GroupService(databaseController);
        }

        #endregion Constructors

        #region Methods

        public void Start()
        {
            Console.WriteLine($"WebSocketServer: {_ip}:{_port}");
            _wsServer.Start();
        }

        public void Stop()
        {
            _wsServer.Stop();
        }

        private void HandleMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string target = e.Target;
            if (!String.IsNullOrEmpty(e.GroupName))
            {
                target = String.Empty;
            }

            if (e.Target == null)
            {
                Console.WriteLine($"{e.Source} : {e.Message}");
            }
            else
            {
                Console.WriteLine($"{e.Source} to {e.Target} : {e.Message}");
            }

            _txtMsgService.AddMessage(e.Source, e.Target, e.Message, e.Date);

            _wsServer.Send(e.Source, target, new MessageBroadcast(e.Source, target, e.Message, DateTime.Now, e.GroupName).GetContainer());
        }

        private void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            string clientState = e.IsConnected ? "подключен" : "отключен";
            string message = $"{e.Client} {clientState}.";

            if (e.IsConnected)
            {
                var chatHistory = _txtMsgService.GetClientMessages(e.Client);
                var clients = _clientService.GetClients();
                var groups = _groupService.GetGroups(e.Client);

                _wsServer.Send(String.Empty, e.Client, new ChatHistoryResponse(chatHistory).GetContainer());
                _wsServer.Send(String.Empty, e.Client, new ClientsListResponse(clients).GetContainer());
                _wsServer.Send(String.Empty, e.Client, new GroupsListResponse(groups).GetContainer());
            }

            _clientEventService.AddClientEvent(MessageType.Event, message, e.Date);

            Console.WriteLine($"{message}");
            
            _wsServer.Send(e.Client, String.Empty, new MessageBroadcast(e.Client, String.Empty, clientState, DateTime.Now, String.Empty).GetContainer());
        }

        private void HandleConnectionReceived(object sender, ConnectionReceivedEventArgs e)
        {
            _clientService.AddClient(e.Login);

            _wsServer.Send(e.Login, String.Empty, new ConnectionBroadcast(e.Login, e.IsConnected, DateTime.Now).GetContainer());
        }

        private void HandleErrorReceived(object sender, ErrorReceivedEventArgs e)
        {
            _clientEventService.AddClientEvent(MessageType.Error, e.Reason, e.Date);
        }

        private void HandleFilterReceived(object sender, FilterReceivedEventArgs e)
        {
            var filteredMessages = _clientEventService.GetClientEvents(e.FirstDate, e.SecondDate, e.MessageTypes);
            _wsServer.Send(String.Empty, e.Login, new FilterResponse(filteredMessages).GetContainer());
        }

        private void HandleCreateGroupReceived(object sender, CreateGroupReceivedEventArgs e)
        {
            _groupService.AddGroup(e.GroupName, e.Clients);
            _wsServer.Send(String.Empty, String.Empty, new GroupBroadcast(new Dictionary<string, List<string>>() { { e.GroupName, e.Clients } }).GetContainer());
        }

        private void HandleLeaveGroupReceived(object sender, LeaveGroupReceivedEventArgs e)
        {
            _groupService.LeaveGroup(e.Source, e.GroupName);
        }

        #endregion Methods
        }
}
