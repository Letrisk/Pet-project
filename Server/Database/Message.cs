namespace Server.Database
{
    using System;

    public class Message
    {
        #region Properties

        public int Id { get; set; }

        public string Source { get; set; }

        public string Target { get; set; }

        public string MessageText { get; set; }

        public DateTime Date { get; set; }

        #endregion Properties
    }
}
