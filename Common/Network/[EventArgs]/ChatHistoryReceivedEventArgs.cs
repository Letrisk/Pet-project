﻿namespace Common.Network
{
    using System.Collections.Generic;

    public class ChatHistoryReceivedEventArgs
    {
        #region Properties

        public Dictionary<string, List<Message>> ClientMessages { get; }

        #endregion Properties

        #region Constructors

        public ChatHistoryReceivedEventArgs(Dictionary<string, List<Message>> clientMessages)
        {
            ClientMessages = clientMessages;
        }

        #endregion Constructors
    }
}
