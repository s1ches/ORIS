using MyProtocolServer.Protocol.XPacketAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProtocolServer.Protocol.XPacketsTypes
{
    public class XPacketHandshake
    {
        [XPacketField(1)]
        public int MagicHandshakeNumber;
    }
}
