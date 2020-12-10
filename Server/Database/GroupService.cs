﻿namespace Server.Database
{
    using System.Collections.Generic;
    using System.Linq;

    public class GroupService
    {
        #region Fields

        private DatabaseController _dbController = new DatabaseController();

        #endregion Fields

        #region Methods

        public void AddGroup(string groupName, List<string> clients)
        {
            _dbController.AddGroup(groupName, clients);
        }

        public Dictionary<string, List<string>> GetGroups(string login)
        {
            Dictionary<string, List<string>> groupsClients = new Dictionary<string, List<string>>();
            
            foreach(var group in _dbController.GetGroups(login))
            {
                groupsClients.Add(group.GroupName, group.Clients.Select(item => item.Login).ToList());
            }
            return groupsClients;
        }

        #endregion Methods
    }
}