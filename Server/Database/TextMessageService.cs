namespace Server.Database
{
    using System;
    using System.Collections.Generic;

    public class TextMessageService
    {
        #region Fields

        private readonly DatabaseController _dbController;

        #endregion Fields

        #region Constructors

        public TextMessageService(DatabaseController databaseController)
        {
            _dbController = databaseController;
        }

        #endregion Constructors

        #region Methods

        public void AddMessage(string source, string target, string message, DateTime date)
        {
            _dbController.AddMessage(source, target, message, date);
        }

        public Dictionary<string, List<Common.Network.Message>> GetClientMessages(string client)
        {
            Dictionary<string, List<Common.Network.Message>> clientMessages = new Dictionary<string, List<Common.Network.Message>>() 
                                                                              { { "General", new List<Common.Network.Message>() } };

            List<Message> messages = _dbController.GetMessageLog(client);

            string message = String.Empty;

            messages.ForEach(msg =>
            {
                message = $"{msg.Date} {msg.Source} : {msg.MessageText}\n";

                if (String.IsNullOrEmpty(msg.Target))
                {
                    clientMessages["General"].Add(new Common.Network.Message(msg.Source, msg.MessageText, client == msg.Source, msg.Date));
                }
                else
                {
                    if (msg.Target == client)
                    {
                        if (!clientMessages.ContainsKey(msg.Source))
                        {
                            clientMessages.Add(msg.Source, new List<Common.Network.Message>());
                        }

                        clientMessages[msg.Source].Add(new Common.Network.Message(msg.Source, msg.MessageText, false, msg.Date));
                    }
                    else
                    {
                        if(msg.Source == client)
                        {
                            if (!clientMessages.ContainsKey(msg.Target))
                            {
                                clientMessages.Add(msg.Target, new List<Common.Network.Message>());
                            }

                            clientMessages[msg.Target].Add(new Common.Network.Message(msg.Source, msg.MessageText, true, msg.Date));
                        }
                        else
                        {
                            if (!clientMessages.ContainsKey(msg.Target))
                            {
                                clientMessages.Add(msg.Target, new List<Common.Network.Message>());
                            }

                            clientMessages[msg.Target].Add(new Common.Network.Message(msg.Source, msg.MessageText, false, msg.Date));
                        }
                        
                    }
                }
            });

            return clientMessages;
        }

        #endregion Methods
    }
}
