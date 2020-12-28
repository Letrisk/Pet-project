namespace Common.Network
{
    using System.Collections.Generic;

    public class CreateGroupReceivedEventArgs
    {
        #region Properties

        public string GroupName { get; }

        public List<string> Clients { get; }

        #endregion Properties

        #region Constructors

        public CreateGroupReceivedEventArgs(string groupName, List<string> clients)
        {
            GroupName = groupName;
            Clients = clients;
        }

        #endregion Constructors
    }
}
