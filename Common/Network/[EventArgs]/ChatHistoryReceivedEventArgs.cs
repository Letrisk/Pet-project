namespace Common.Network
{
    using System.Collections.Generic;

    public class ChatHistoryReceivedEventArgs
    {
        #region Properties

        public Dictionary<string, List<string>> ClientMessages { get; }

        #endregion Properties

        #region Constructors

        public ChatHistoryReceivedEventArgs(Dictionary<string, List<string>> clientMessages)
        {
            ClientMessages = clientMessages;
        }

        #endregion Constructors
    }
}
