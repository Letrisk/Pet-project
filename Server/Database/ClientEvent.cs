namespace Server.Database
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    using Common.Network;

    public class ClientEvent
    {
        #region Properties

        public int Id { get; set; }

        public MessageType MessageType { get; set; }

        public string Message { get; set; }

        public DateTime Date { get; set; }

        #endregion Properties
    }
}
