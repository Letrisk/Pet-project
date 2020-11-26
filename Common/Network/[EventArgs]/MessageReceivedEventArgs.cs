namespace Common.Network._EventArgs_
{
    using System;

    public class MessageReceivedEventArgs
    {
        #region Properties

        public string Target { get; }

        public string Source { get; }

        public string Message { get; }

        public DateTime Date { get; }

        #endregion Properties

        #region Constructors

        public MessageReceivedEventArgs(string source, string target, string message, DateTime date)
        {
            Source = source;
            Target = target;
            Message = message;
            Date = date;
        }

        #endregion Constructors
    }
}
