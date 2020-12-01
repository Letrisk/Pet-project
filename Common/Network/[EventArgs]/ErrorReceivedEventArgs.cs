namespace Common.Network
{
    using System;

    public class ErrorReceivedEventArgs
    {
        #region Properties

        public string Reason { get; }

        public DateTime Date { get; }

        #endregion Properties

        #region Constructors

        public ErrorReceivedEventArgs(string reason, DateTime date)
        {
            Reason = reason;
            Date = date;
        }

        #endregion Constructors
    }
}
