namespace Common.Network.Messages
{
    using System.Collections.Generic;

    public class ClientsListResponse
    {
        #region Properties

        public List<string> Clients { get; set; }

        #endregion Properties

        #region Constructors

        public ClientsListResponse(List<string> clients)
        {
            Clients = clients;
        }

        #endregion Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(ClientsListResponse),
                Payload = this
            };

            return container;
        }

        #endregion Methods
    }
}
