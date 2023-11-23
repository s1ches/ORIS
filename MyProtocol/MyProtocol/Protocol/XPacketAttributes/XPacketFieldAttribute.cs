using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProtocolServer.Protocol.XPacketAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class XPacketFieldAttribute : Attribute
    {
        public byte FieldID { get; }
        public XPacketFieldAttribute(byte fieldId)
        {
            FieldID = fieldId;
        }
    }
}
