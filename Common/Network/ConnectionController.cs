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

        private readonly IController _controller;

        #endregion Fields

        #region Events

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
        public event EventHandler<ErrorReceivedEventArgs> ErrorReceived;

        #endregion Events

        #region Constructors

        public ConnectionController(IController controller)
        {
            _controller = controller;

            _controller.ConnectionStateChanged += HandleConnectionStateChanged;
            _controller.ErrorReceived += HandleErrorReceived;
        }

        #endregion Constructors

        #region Methods

        public void Connect(string address, string port)
        {
            _controller.Connect(address, port);
        }

        public void Disconnect()
        {
            _controller.Disconnect();
        }

        public void Login(string login)
        {            
            _controller.Login = login;
            _controller.Send(new ConnectionRequest(login).GetContainer());
        }

        public void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, e);
        }

        public void HandleErrorReceived(object sender, ErrorReceivedEventArgs e)
        {
            ErrorReceived?.Invoke(this, e);
        }

        #endregion Methods
    }
}
