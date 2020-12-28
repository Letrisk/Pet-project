namespace Common.Network.Messages
{
    public class MessageRequest
    {
        #region Properties

        public string Message { get; set; }
        public string Target { get; set; }
        public string GroupName { get; set; }

        #endregion Properties

        #region Constructors

        public MessageRequest(string target, string message, string groupName)
        {
            Target = target;
            Message = message;
            GroupName = groupName;
        }

        #endregion Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(MessageRequest),
                Payload = this
            };

            return container;
        }

        #endregion Methods
    }
}
