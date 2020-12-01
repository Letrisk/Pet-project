namespace Server.Database
{
    using System;
    using System.Collections.Generic;

    public class TextMessageService
    {
        #region Fields

        private DatabaseController _dbController = new DatabaseController();

        #endregion Fields

        #region Methods

        public void AddMessage(string source, string target, string message, DateTime date)
        {
            _dbController.AddMessage(source, target, message, date);
        }

        public Dictionary<string, string> GetClientMessages(string client)
        {
            Dictionary<string, string> clientMessages = new Dictionary<string, string>() { { "General", String.Empty } };

            List<Message> messages = _dbController.GetMessageLog(client);

            string message = String.Empty;

            foreach(Message msg in messages)
            {
                message = $"{msg.Date} {msg.Source} : {msg.MessageText}\n";
                if (msg.Source != client)
                {
                    if (!clientMessages.ContainsKey(msg.Source))
                    {
                        clientMessages.Add(msg.Source, String.Empty);
                    }

                    clientMessages[msg.Source] += message;
                }
                else
                {
                    if (msg.Target != client)
                    {
                        if (String.IsNullOrEmpty(msg.Target))
                        {
                            clientMessages["General"] += message;
                        }
                        else
                        {
                            if (!clientMessages.ContainsKey(msg.Target))
                            {
                                clientMessages.Add(msg.Target, String.Empty);
                            }

                            clientMessages[msg.Target] += message;
                        }
                    }
                    else
                    {
                        if (!clientMessages.ContainsKey(client))
                        {
                            clientMessages.Add(client, String.Empty);
                        }

                        clientMessages[client] += message;
                    }
                }
            }

            return clientMessages;
        }

        #endregion Methods
    }
}
