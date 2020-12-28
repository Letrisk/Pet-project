namespace Common.Network
{
    using System;

    public class Message
    {
        #region Properties

        public string Source { get; set; }

        public string Text { get; set; }

        //переменная необходима для стиля
        public bool IsYourMessage { get; set; }

        public DateTime DateTime { get; set; }

        #endregion Properties

        #region Constructors

        public Message(string source, string text, bool isYourMessage, DateTime dateTime)
        {
            Source = source;
            Text = text;
            IsYourMessage = isYourMessage;
            DateTime = dateTime;
        }

        #endregion Constructors
    }
}
