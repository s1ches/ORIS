﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProtocolServer.Protocol.XPacket
{
    public class XPacketField
    {
        public byte FieldID { get; set; }
        public byte FieldSize { get; set; }
        public byte[]? Contents { get; set; }
    }
}
