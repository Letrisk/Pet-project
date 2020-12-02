namespace Common.Network
{
    public class FilteredMessagesReceivedEventArgs
    {
        #region Properties

        public string FilteredMessages { get; }

        #endregion Properties

        #region Constructors

        public FilteredMessagesReceivedEventArgs(string filteredMessages)
        {
            FilteredMessages = filteredMessages;
        }

        #endregion Constructors
    }
}
