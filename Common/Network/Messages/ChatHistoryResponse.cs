namespace Common.Network.Messages
{
    using System;
    using System.Collections.Generic;

    public class ChatHistoryResponse
    {
        #region Properties

        public Dictionary<string, string> ClientMessages { get; set; }

        #endregion Properties

        #region Constructors

        public ChatHistoryResponse(Dictionary<string, string> clientMessages)
        {
            ClientMessages = clientMessages;
        }

        #endregion Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(ChatHistoryResponse),
                Payload = this
            };

            return container;
        }

        #endregion Methods
    }
}
