namespace Common.Network
{
    using System;
    using System.Collections.ObjectModel;

    public class ConnectionReceivedEventArgs
    {
        #region Properties

        public string Login { get; }

        public bool IsConnected { get; }

        public DateTime Date { get; }

        #endregion Properties

        #region Constructors

        public ConnectionReceivedEventArgs(string login, bool isConnected, DateTime date)
        {
            Login = login;
            IsConnected = isConnected;
            Date = date;
        }

        #endregion Constructors
    }
}
