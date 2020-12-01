namespace Server.Database
{
    using System;
    using System.Collections.Generic;

    using Common.Network;

    public class ClientEventService
    {
        #region Fields

        private DatabaseController _dbController = new DatabaseController();

        #endregion Fields

        #region Methods

        public void AddClientEvent(MessageType messageType, string message, DateTime date)
        {
            _dbController.AddClientEvent(messageType, message, date);
        }

        /*public string GetClientEvents(DateTime firstDate, DateTime secondDate, string[] messageTypes)
        {

        }*/

        #endregion Methods
    }
}
