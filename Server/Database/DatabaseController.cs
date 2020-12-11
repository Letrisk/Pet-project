namespace Server.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity;
    using System.Threading;
    using System.Threading.Tasks;

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

            using (var context = new DatabaseContext())
            {
                context.Messages.Add(message);
                context.SaveChanges();
            }
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

        public void AddGroup(string groupName, List<string> clients)
        {
            Group group = new Group { GroupName = groupName, Clients = new List<Client>() };

            try
            {
                foreach (var client in clients)
                {
                    _dbContext.Clients.Find(client).Groups.Add(group);
                }

                _dbContext.Groups.Add(group);
                _dbContext.SaveChanges();
            }
            catch(Exception ex)
            {
                string errorMessage = "Такая группа уже существует";
                Console.WriteLine(errorMessage);

                _dbContext.EventLog.Add(new ClientEvent
                {
                    Date = DateTime.Now,
                    MessageType = MessageType.Error,
                    Message = errorMessage
                });
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

        public List<Message> GetMessageLog(string login)
        {
            List<Message> messageLog = new List<Message>();
            List<string> groups = GetGroups(login).Select(item => item.GroupName).ToList();

            var messages = _dbContext.Messages.Where(m => m.Source == login || m.Target == login || String.IsNullOrEmpty(m.Target) || 
                                                          groups.Contains(m.Target));

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

        public List<Group> GetGroups(string login)
        {
            List<Group> groups = new List<Group>();

            var clientGroups = _dbContext.Groups.Where(item => item.Clients.Where(client => client.Login == login).Count() != 0);

            return (groups = clientGroups.ToList());
        }

        public List<Client> GetClients()
        {
            List<Client> clients = _dbContext.Clients.ToList();

            return clients;
        }

        #endregion Properties
    }
}
