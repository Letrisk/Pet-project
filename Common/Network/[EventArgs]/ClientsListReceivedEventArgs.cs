namespace Common.Network
{
    using System.Collections.Generic;

    public class ClientsListReceivedEventArgs
    {
        #region Properties

        public List<string> ClientsList { get; }

        #endregion Properties

        #region Constructors

        public ClientsListReceivedEventArgs(List<string> clientsList)
        {
            ClientsList = clientsList;
        }

        #endregion Constructors
    }
}
