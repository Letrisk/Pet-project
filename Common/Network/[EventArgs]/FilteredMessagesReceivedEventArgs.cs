namespace Common.Network
{
    using System.Collections.Generic;

    public class FilteredMessagesReceivedEventArgs
    {
        #region Properties

        public List<string> FilteredMessages { get; }

        #endregion Properties

        #region Constructors

        public FilteredMessagesReceivedEventArgs(List<string> filteredMessages)
        {
            FilteredMessages = filteredMessages;
        }

        #endregion Constructors
    }
}
