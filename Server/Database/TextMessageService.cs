namespace Server.Database
{
    using System;
    using System.Collections.Generic;

    public class TextMessageService
    {
        #region Fields

        private DatabaseController _dbController;

        #endregion Fields

        #region Constructors

        public TextMessageService()
        {
            _dbController = new DatabaseController();
        }

        #endregion Constructors

        #region Methods

        public void AddMessage(string source, string target, string message, DateTime date)
        {
            _dbController.AddMessage(source, target, message, date);
        }

        public Dictionary<string, List<string>> GetClientMessages(string client)
        {
            Dictionary<string, List<string>> clientMessages = new Dictionary<string, List<string>>() { { "General", new List<string>() } };

            List<Message> messages = _dbController.GetMessageLog(client);

            string message = String.Empty;

            messages.ForEach(msg =>
            {
                message = $"{msg.Date} {msg.Source} : {msg.MessageText}\n";

                if (String.IsNullOrEmpty(msg.Target))
                {
                    clientMessages["General"].Add(message);
                }
                else
                {
                    if (msg.Target == client)
                    {
                        if (!clientMessages.ContainsKey(msg.Source))
                        {
                            clientMessages.Add(msg.Source, new List<string>());
                        }

                        clientMessages[msg.Source].Add(message);
                    }
                    else
                    {
                        if (!clientMessages.ContainsKey(msg.Target))
                        {
                            clientMessages.Add(msg.Target, new List<string>());
                        }

                        clientMessages[msg.Target].Add(message);
                    }
                }
            });

            return clientMessages;
        }

        #endregion Methods
    }
}
