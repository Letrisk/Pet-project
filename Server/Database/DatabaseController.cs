namespace Server.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity;

    using Common.Network;

    public class DatabaseController
    {
        #region Fields

        private readonly MessageContext _messageDb = new MessageContext();
        private readonly ClientEventContext _clientEventDb = new ClientEventContext();
        private readonly ClientContext _clientDb = new ClientContext();

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

            try
            {
                _clientEventDb.EventLog.Add(clientEvent);
                _clientEventDb.SaveChanges();
            }
            catch (NullReferenceException)
            {
                string errorMessage = "Пользователь слишком быстро отключился";
                Console.WriteLine(errorMessage);

                _clientEventDb.EventLog.Add(new ClientEvent
                {
                    Date = DateTime.Now,
                    MessageType = MessageType.Error,
                    Message = errorMessage
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void AddClient(string login)
        {
            try
            {
                if (_clientDb.Clients.Find(login) == null)
                {
                    Client client = new Client { Login = login };

                    _clientDb.Clients.Add(client);
                    _clientDb.SaveChanges();
                }
            }
            catch(NullReferenceException)
            {
                string errorMessage = "Пользователь слишком быстро отключился";
                Console.WriteLine(errorMessage);

                _clientEventDb.EventLog.Add(new ClientEvent
                {
                    Date = DateTime.Now,
                    MessageType = MessageType.Error,
                    Message = errorMessage
                });
            }           
        }

        public List<Message> GetMessageLog(string source)
        {
            List<Message> messageLog = new List<Message>();

            var messages = _messageDb.Messages.Where(m => m.Source == source || m.Target == source || String.IsNullOrEmpty(m.Target));

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

        public List<Client> GetClients()
        {
            List<Client> clients = _clientDb.Clients.ToList();

            return clients;
        }

        #endregion Properties
    }
}
