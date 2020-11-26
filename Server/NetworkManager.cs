namespace Server
{
    using System;
    using System.Net;

    using Common.Network;
    using Common.Network._EventArgs_;

    public class NetworkManager
    {
        #region Constants

        private const int WS_PORT = 65000;
        private const int TCP_PORT = 65001;

        #endregion Constants

        #region Fields

        private readonly WsServer _wsServer;

        #endregion Fields

        #region Constructors

        public NetworkManager()
        {
            _wsServer = new WsServer(new IPEndPoint(IPAddress.Any, WS_PORT));
            _wsServer.ConnectionStateChanged += HandleConnectionStateChanged;
            _wsServer.ConnectionReceived += HandleConnectionReceived;
            _wsServer.MessageReceived += HandleMessageReceived;
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
            string message = $"{e.Message}";

            if (e.Target == null)
            {
                Console.WriteLine($"{e.Source} : {message}");
            }
            else
            {
                Console.WriteLine($"{e.Source} to {e.Target} : {e.Message}");
            }
            _wsServer.SendMessageBroadcast(e.Source, e.Target, message);
        }

        private void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            string clientState = e.IsConnected ? "подключен" : "отключен";
            string message = $"{clientState}.";

            Console.WriteLine($"{e.Client} {message}");
            
            _wsServer.SendMessageBroadcast(e.Client, null, message);
        }

        private void HandleConnectionReceived(object sender, ConnectionReceivedEventArgs e)
        {
            _wsServer.SendConnectionBroadcast(e.Login, e.IsConnected);
        }

            #endregion Methods
        }
}
