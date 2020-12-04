namespace Server.Database
{
    using System.Collections.Generic;
    using System.Linq;

    public class ClientService
    {
        #region Fields

        private DatabaseController _dbController = new DatabaseController();

        #endregion Fields

        #region Methods

        public void AddClient(string login)
        {
            _dbController.AddClient(login);
        }

        public List<string> GetClients()
        {
            List<string> clients = new List<string>();

            List<Client> clientList = _dbController.GetClients();

            clients = clientList.Select(item => item.Login).ToList();

            return clients;
        }

        #endregion Methods
    }
}
