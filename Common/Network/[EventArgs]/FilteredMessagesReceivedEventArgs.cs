namespace Common.Network
{
    using System.Collections.Generic;

    public class FilteredMessagesReceivedEventArgs
    {
        #region Properties

        public List<Message> FilteredMessages { get; }

        #endregion Properties

        #region Constructors

        public FilteredMessagesReceivedEventArgs(List<Message> filteredMessages)
        {
            FilteredMessages = filteredMessages;
        }

        #endregion Constructors
    }
}
