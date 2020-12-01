namespace Server.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common.Network;

    public class DatabaseController
    {
        #region Fields

        private readonly MessageContext _messageDb = new MessageContext();
        private readonly ClientEventContext _clientEventDb = new ClientEventContext();

        #endregion Fields
        
        #region Properties

        public void AddMessage(string source, string target, string messageText, DateTime date)
        {
                Message message = new Message { Source = source, Target = target, MessageText = messageText, Date = date };

                _messageDb.Messages.Add(message);
                _messageDb.SaveChanges();
        }

        public void AddClientEvent(MessageType messageType, string message, DateTime date)
        {
            ClientEvent clientEvent = new ClientEvent { MessageType = messageType, Message = message, Date = date };

            _clientEventDb.EventLog.Add(clientEvent);
            _clientEventDb.SaveChanges();
        }

        public List<Message> GetMessageLog(string source)
        {
            List<Message> messageLog = new List<Message>();

            var messages = _messageDb.Messages.Where(m => m.Source == source || m.Target == source);

            foreach(Message msg in messages)
            {
                messageLog.Add(msg);
            }

            return messageLog;
        }

        public List<ClientEvent> GetClientEventLog(DateTime firstDate, DateTime secondDate, string[] messageTypes)
        {
            List<ClientEvent> clientEventLog = new List<ClientEvent>();

            var clientEvents = _clientEventDb.EventLog.Where(e => e.Date >= firstDate && e.Date <= secondDate && messageTypes.Contains(e.MessageType.ToString()));

            return (clientEventLog = clientEvents.ToList<ClientEvent>());
        }

        #endregion Properties
    }
}
