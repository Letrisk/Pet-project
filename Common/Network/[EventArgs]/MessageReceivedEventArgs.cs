namespace Common.Network
{
    using System;

    public class MessageReceivedEventArgs
    {
        #region Properties

        public string Target { get; }

        public string Source { get; }

        public string Message { get; }

        public string GroupName { get; }

        public DateTime Date { get; }

        #endregion Properties

        #region Constructors

        public MessageReceivedEventArgs(string source, string target, string message, DateTime date, string groupName = null)
        {
            Source = source;
            Target = target;
            Message = message;
            GroupName = groupName;
            Date = date;
        }

        #endregion Constructors
    }
}
