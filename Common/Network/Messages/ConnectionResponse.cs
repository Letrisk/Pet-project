namespace Common.Network.Messages
{
    using System;

    using Common.Network;
    class ConnectionResponse
    {
        #region Properties

        public ResultCodes Result { get; set; }

        public string Reason { get; set; }

        public bool IsSuccessful { get; set; }

        public DateTime Date { get; set; }

        public string[] OnlineClients { get; set; }

        #endregion Properties

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(ConnectionResponse),
                Payload = this
            };

            return container;
        }

        #endregion Methods
    }
}
