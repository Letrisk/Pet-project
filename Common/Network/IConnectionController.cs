namespace Common.Network
{
    using System;

    using WebSocketSharp;

    public interface IConnectionController
    {
        #region Events

        event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        #endregion Events

        #region Methods

        void Connect(string address, string port);

        void Disconnect();

        void Login(string login);

        #endregion Methods
    }
}
