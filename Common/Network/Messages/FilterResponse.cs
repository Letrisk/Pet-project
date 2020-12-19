namespace Common.Network.Messages
{
    using System.Collections.Generic;

    public class FilterResponse
    {
        #region Properties

        public List<string> FilteredMessages { get; set; }

        #endregion Properties

        #region Constructors

        public FilterResponse(List<string> filteredMessages)
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
