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

        private readonly DatabaseContext _dbContext = new DatabaseContext();

        #endregion Fields
        
        #region Properties

        public void AddMessage(string source, string target, string messageText, DateTime date)
        {
            Message message = new Message { Source = source, Target = target, MessageText = messageText, Date = date };

            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();
        }

        public void AddClientEvent(MessageType messageType, string message, DateTime date)
        {
            ClientEvent clientEvent = new ClientEvent { MessageType = messageType, Message = message, Date = date };

            try
            {
                _dbContext.EventLog.Add(clientEvent);
                _dbContext.SaveChanges();
            }
            catch (NullReferenceException)
            {
                string errorMessage = "Пользователь слишком быстро отключился";
                Console.WriteLine(errorMessage);

                _dbContext.EventLog.Add(new ClientEvent
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
                if (_dbContext.Clients.Find(login) == null)
                {
                    Client client = new Client { Login = login };

                    _dbContext.Clients.Add(client);
                    _dbContext.SaveChanges();
                }
            }
            catch(NullReferenceException)
            {
                string errorMessage = "Пользователь слишком быстро отключился";
                Console.WriteLine(errorMessage);

                _dbContext.EventLog.Add(new ClientEvent
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

            var messages = _dbContext.Messages.Where(m => m.Source == source || m.Target == source || String.IsNullOrEmpty(m.Target));

            foreach(Message msg in messages)
            {
                messageLog.Add(msg);
            }

            return messageLog;
        }

        public List<ClientEvent> GetClientEventLog(DateTime firstDate, DateTime secondDate, string[] messageTypes)
        {
            List<ClientEvent> clientEventLog = new List<ClientEvent>();

            var clientEvents = _dbContext.EventLog.Where(e => e.Date >= firstDate && e.Date <= secondDate && messageTypes.Contains(e.MessageType.ToString()));

            return (clientEventLog = clientEvents.ToList<ClientEvent>());
        }

        public List<Client> GetClients()
        {
            List<Client> clients = _dbContext.Clients.ToList();

            return clients;
        }

        #endregion Properties
    }
}
