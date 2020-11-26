namespace Common.Network._EventArgs_
{
    using System;
    public class ConnectionStateChangedEventArgs
    {
        #region Properties

        public string Client { get; }

        public DateTime Date { get; }

        public bool IsConnected { get; }

        public string[] OnlineClients { get; }

        #endregion Properties

        #region Constructors

        public ConnectionStateChangedEventArgs(string client, DateTime date, bool isConnected, string[] onlineClients = null)
        {
            Client = client;
            Date = date;
            IsConnected = isConnected;
            OnlineClients = onlineClients;
        }

        #endregion Constructors
    }
}
