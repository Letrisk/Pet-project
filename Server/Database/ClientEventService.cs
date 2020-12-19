namespace Server.Database
{
    using System;
    using System.Collections.Generic;

    using Common.Network;

    public class ClientEventService
    {
        #region Fields

        private DatabaseController _dbController;

        #endregion Fields

        #region Constructors

        public ClientEventService()
        {
            _dbController = new DatabaseController();
        }

        #endregion Constructors

        #region Methods

        public void AddClientEvent(MessageType messageType, string message, DateTime date)
        {
            _dbController.AddClientEvent(messageType, message, date);
        }

        public List<string> GetClientEvents(DateTime firstDate, DateTime secondDate, MessageType messageTypes)
        {
            List<string> clientEvents = new List<string>();

            List<ClientEvent> clientEventsList = _dbController.GetClientEventLog(firstDate, secondDate, messageTypes);

            foreach (ClientEvent clientEvent in clientEventsList)
            {
                clientEvents.Add($"{clientEvent.Date} {clientEvent.MessageType} : {clientEvent.Message}\n");
            }

            return clientEvents;
        }

        #endregion Methods
    }
}
