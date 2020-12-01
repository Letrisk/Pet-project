namespace Common.Network
{
    using System;

    public class FilterReceivedEventArgs
    {
        #region Properties

        public DateTime FirstDate { get; }
        public DateTime SecondDate { get; }

        public string[] MessageTypes { get; }

        #endregion Properties

        #region Constructors

        public FilterReceivedEventArgs(DateTime firstDate, DateTime secondDate, string[] messageTypes)
        {
            FirstDate = firstDate;
            SecondDate = secondDate;
            MessageTypes = messageTypes;
        }

        #endregion Constructors
    }
}
