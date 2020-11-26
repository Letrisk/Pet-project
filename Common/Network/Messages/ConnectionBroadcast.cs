namespace Common.Network.Messages
{
    using System;

    public class ConnectionBroadcast
    {
        #region Properties

        public string Login { get; set; }

        public bool IsConnected { get; set; }

        public DateTime Date { get; set; }

        #endregion Properties

        #region Constructors

        public ConnectionBroadcast(string login, bool isConnected, DateTime date)
        {
            Login = login;
            IsConnected = isConnected;
            Date = date;
        }

        #endregion Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(ConnectionBroadcast),
                Payload = this
            };

            return container;
        }

        #endregion Methods
    }
}
