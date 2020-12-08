﻿namespace Server
{
    using System;
    using System.Net;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Data.SqlClient;
    using System.Configuration;
    using System.Linq;
    using System.IO;

    using Common.Network;

    using Database;

    public class NetworkManager
    {
        #region Fields

        private string _transport;
        private int _port, _timeout;
        private IPAddress _ip;
        private ConnectionStringSettings _connectionSettings;

        private readonly WsServer _wsServer;

        private TextMessageService _txtMsgService = new TextMessageService();
        private ClientEventService _clientEventService = new ClientEventService();
        private ClientService _clientService = new ClientService();

        #endregion Fields

        #region Constructors

        public NetworkManager()
        {
            SettingsManager settingsManager = new SettingsManager("ServerConfig.xml");
            _transport = settingsManager.Transport;
            _ip = settingsManager.Ip;
            _port = settingsManager.Port;
            _timeout = settingsManager.Timeout;


            if (_transport == "WebSocket")
            {
                _wsServer = new WsServer(new IPEndPoint(_ip, _port));
                _wsServer.ConnectionStateChanged += HandleConnectionStateChanged;
                _wsServer.ConnectionReceived += HandleConnectionReceived;
                _wsServer.MessageReceived += HandleMessageReceived;
                _wsServer.ErrorReceived += HandleErrorReceived;
                _wsServer.FilterReceived += HandleFilterReceived;
                _wsServer.Timeout = _timeout;
            }
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
                var clients = _clientService.GetClients();

                _wsServer.SendChatHistory(e.Client, chatHistory);
                _wsServer.SendClientsList(e.Client, clients);
            }

            _clientEventService.AddClientEvent(MessageType.Event, message, e.Date);

            Console.WriteLine($"{message}");
            
            _wsServer.SendMessageBroadcast(e.Client, null, clientState);
        }

        private void HandleConnectionReceived(object sender, ConnectionReceivedEventArgs e)
        {
            _clientService.AddClient(e.Login);

            _wsServer.SendConnectionBroadcast(e.Login, e.IsConnected);
        }

        private void HandleErrorReceived(object sender, ErrorReceivedEventArgs e)
        {
            _clientEventService.AddClientEvent(MessageType.Error, e.Reason, e.Date);
        }

        private void HandleFilterReceived(object sender, FilterReceivedEventArgs e)
        {
            var filteredMessages = _clientEventService.GetClientEvents(e.FirstDate, e.SecondDate, e.MessageTypes);
            _wsServer.SendFilteredMessages(e.Login, filteredMessages);
        }

        #endregion Methods
        }
}
