using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network.Messages
{
    public class MessageContainer
    {
        #region Properties

        public string Identifier { get; set; }

        public object Payload { get; set; }

        #endregion Properties
    }
}
