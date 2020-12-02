namespace Common.Network.Messages
{
    public class FilterResponse
    {
        #region Properties

        public string FilteredMessages { get; set; }

        #endregion Properties

        #region Constructors

        public FilterResponse(string filteredMessages)
        {
            FilteredMessages = filteredMessages;
        }

        #endregion Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(FilterResponse),
                Payload = this
            };

            return container;
        }

        #endregion Methods
    }
}
