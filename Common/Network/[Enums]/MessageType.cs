using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    [Flags]
    public enum MessageType
    {
        Message = 1,
        Event = 2,
        Error = 4
    }
}
