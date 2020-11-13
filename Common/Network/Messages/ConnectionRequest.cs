using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network.Messages
{
    class ConnectionRequest
    {
        #region Properties

        public string Login { get; set; }

        #endregion Properties

        #region Constructors

        public ConnectionRequest(string login)
        {
            Login = login;
        }

        #endregion Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(ConnectionRequest),
                Payload = this
            };

            return container;
        }

        #endregion Methods
    }
}
