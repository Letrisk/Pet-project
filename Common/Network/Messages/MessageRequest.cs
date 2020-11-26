namespace Common.Network.Messages
{
    public class MessageRequest
    {
        #region Properties

        public string Message { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }

        #endregion Properties

        #region Constructors

        public MessageRequest(string source, string target, string message)
        {
            Source = source;
            Target = target;
            Message = message;
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
