using MyProtocolServer.Protocol.XPacketAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProtocolServer.Protocol.XPacketsTypes
{
    public class XPacketMessage
    {
        [XPacketField(1)]
        public byte[] From;

        [XPacketField(2)]
        public byte[] To;

        [XPacketField(3)]
        public byte[] DateTime;

        [XPacketField(4)]
        public byte[] Message;
    }
}
