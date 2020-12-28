namespace Common.Network.Messages
{
    using System.Collections.Generic;

    public class CreateGroupRequest
    {
        #region Properties

        public string GroupName { get; set; }

        public List<string> Clients { get; set; }

        #endregion Properties

        #region Constructors

        public CreateGroupRequest(string groupName, List<string> clients)
        {
            GroupName = groupName;
            Clients = clients;
        }

        #endregion Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(CreateGroupRequest),
                Payload = this
            };

            return container;
        }

        #endregion Methods
    }
}
