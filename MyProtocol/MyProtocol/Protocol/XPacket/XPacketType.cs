using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProtocolServer.Protocol.XPacket
{
    public enum XPacketType
    {
        Unknown,
        Handshake,
        Message
    }
}
