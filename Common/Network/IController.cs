namespace Common.Network
{
    using System;

    using _EventArgs_;

    public interface IController
    {
        #region Events

        event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
        event EventHandler<ConnectionReceivedEventArgs> ConnectionReceived;
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion Events

        #region Methods

        void Connect(string address, string port);

        void Disconnect();

        void Login(string login);

        void Send(string source, string target, string message);

        #endregion Methods
    }
}
