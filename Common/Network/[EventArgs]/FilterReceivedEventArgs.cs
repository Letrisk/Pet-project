namespace Common.Network
{
    using System;

    public class FilterReceivedEventArgs
    {
        #region Properties

        public string Login { get; }

        public DateTime FirstDate { get; }
        public DateTime SecondDate { get; }

        public string[] MessageTypes { get; }

        #endregion Properties

        #region Constructors

        public FilterReceivedEventArgs(string login, DateTime firstDate, DateTime secondDate, string[] messageTypes)
        {
            Login = login;
            FirstDate = firstDate;
            SecondDate = secondDate;
            MessageTypes = messageTypes;
        }

        #endregion Constructors
    }
}
