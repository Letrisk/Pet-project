namespace Server.Database
{
    using System;
    using System.Collections.Generic;

    using Common.Network;

    public class ClientEventService
    {
        #region Fields

        private readonly DatabaseController _dbController;

        #endregion Fields

        #region Constructors

        public ClientEventService(DatabaseController databaseController)
        {
            _dbController = databaseController;
        }

        #endregion Constructors

        #region Methods

        public void AddClientEvent(MessageType messageType, string message, DateTime date)
        {
            _dbController.AddClientEvent(messageType, message, date);
        }

        public List<Common.Network.Message> GetClientEvents(DateTime firstDate, DateTime secondDate, MessageType messageTypes)
        {
            List<Common.Network.Message> clientEvents = new List<Common.Network.Message>();

            List<ClientEvent> clientEventsList = _dbController.GetClientEventLog(firstDate, secondDate, messageTypes);

            foreach (ClientEvent clientEvent in clientEventsList)
            {
                string message = $"{clientEvent.MessageType} : {clientEvent.Message}";
                clientEvents.Add(new Common.Network.Message("Event log", message, false, clientEvent.Date));
            }

            return clientEvents;
        }

        #endregion Methods
    }
}
