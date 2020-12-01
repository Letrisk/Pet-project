namespace Common.Network.Messages
{
    using System;

    public class FilterRequest
    {
        #region Properties

        public DateTime FirstDate { get; set; }
        public DateTime SecondDate { get; set; }

        public string[] MessageTypes { get; set; }

        #endregion Properties

        #region Constructors

        public FilterRequest(DateTime firstDate, DateTime secondDate, string[] messageType)
        {
            FirstDate = firstDate;
            SecondDate = secondDate;
            MessageTypes = messageType;
        }

        #endregion Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(ConnectionRequest),
                Payload = this
            };

            return container;
        }

        #endregion Methods
    }
}
