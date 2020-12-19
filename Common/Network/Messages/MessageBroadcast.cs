namespace Common.Network.Messages
{
    using System;

    public class MessageBroadcast
    {
        #region Properties

        public string Message { get; set; }

        public string Source { get; set; }

        public string Target { get; set; }

        public DateTime Date { get; set; }

        public string GroupName { get; set; }

        #endregion Properties

        #region Constructors

        public MessageBroadcast(string source, string target, string message, DateTime date, string groupName)
        {
            Message = message;
            Source = source;
            Target = target;
            Date = date;
            GroupName = groupName;
        }

        #endregion Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(MessageBroadcast),
                Payload = this
            };

            return container;
        }

        #endregion Methods
    }
}
