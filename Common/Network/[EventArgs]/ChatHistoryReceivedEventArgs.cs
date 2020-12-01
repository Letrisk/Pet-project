namespace Common.Network
{
    using System;
    using System.Collections.Generic;

    public class ChatHistoryReceivedEventArgs
    {
        #region Properties

        public Dictionary<string, string> ClientMessages { get; }

        #endregion Properties

        #region Constructors

        public ChatHistoryReceivedEventArgs(Dictionary<string, string> clientMessages)
        {
            ClientMessages = clientMessages;
        }

        #endregion Constructors
    }
}
