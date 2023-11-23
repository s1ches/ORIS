using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyProtocolServer.Protocol.XPacket
{
    public class XPacket
    {
        public byte PacketType { get; private set; }
        public byte PacketSubtype { get; private set; }
        public List<XPacketField> Fields { get; set; } = new();

        private XPacket()
        {
        }

        public static XPacket Create(byte type, byte subtype)
        {
            return new() { PacketType = type, PacketSubtype = subtype };
        }

        public byte[] ToPacket()
        {
            using (var packet = new MemoryStream())
            {

                packet.Write(
                new byte[] { 0xAF, 0xAA, 0xAF, PacketType, PacketSubtype }, 0, 5);

                var fields = Fields.OrderBy(field => field.FieldID);

                foreach (var field in fields)
                {
                    packet.Write(new[] { field.FieldID, field.FieldSize }, 0, 2);
                    packet.Write(field.Contents!, 0, field.Contents!.Length);
                }

                packet.Write(new byte[] { 0xFF, 0x00 }, 0, 2);

                return packet.ToArray();
            }
        }

        public static XPacket? Parse(byte[] packet)
        {
            if (packet.Length < 7) return null;
            if (packet[0] != 0xAF || packet[1] != 0xAA || packet[2] != 0xAF) return null;

            var mIndex = packet.Length - 1;

            if (packet[mIndex - 1] != 0xFF || packet[mIndex - 2] != 0x00) return null;

            var type = packet[3];
            var subtype = packet[4];

            var xPacket = Create(type, subtype);

            var fields = packet.Skip(5).ToArray();

            while (fields.Length != 2)
            {
                var id = fields[0];
                var size = fields[1];

                var contents = size != 0 ? fields.Skip(2).Take(size).ToArray() : null;

                xPacket.Fields.Add(new()
                {
                    FieldID = id,
                    FieldSize = size,
                    Contents = contents
                });

                fields = fields.Skip(2 + size).ToArray();
            }

            return xPacket;
        }

        public byte[] FixedObjectToByteArray(object value)
        {
            var rawsize = Marshal.SizeOf(value);
            var rawdata = new byte[rawsize];

            var handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);

            Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);

            handle.Free();

            return rawdata;
        }

        private T ByteArrayToFixedObject<T>(byte[] bytes) where T : struct
        {
            T structure;

            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try
            {
                structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }

            return structure;
        }

        public XPacketField? GetField(byte id)
        {
            return Fields.FirstOrDefault(field => field.FieldID == id);
        }

        public bool HasField(byte id) => GetField(id) is not null;

        public T GetValue<T>(byte id) where T : struct
        {
            var field = GetField(id);

            if (field is null)
                throw new Exception($"Field with ID {id} wasn't found.");

            var neededSize = Marshal.SizeOf(typeof(T));

            if (field.FieldSize != neededSize)
                throw new Exception($"Can't convert field to type {typeof(T).FullName}.\n"
                    + $"We have {field.FieldSize} bytes but we need exactly {neededSize}.");

            return ByteArrayToFixedObject<T>(field.Contents!);
        }

        public void SetValue(byte id, object structure)
        {
            if (!structure.GetType().IsValueType)
                throw new Exception("Only value types are available.");

            var field = GetField(id);

            if (field is null)
            {
                field = new() { FieldID = id };
                Fields.Add(field);
            }

            var bytes = FixedObjectToByteArray(structure);

            if (bytes.Length > byte.MaxValue)
                throw new Exception("Object is too big. Max length is 255 bytes.");

            field.FieldSize = (byte)bytes.Length;
            field.Contents = bytes;
        }
    }
}
