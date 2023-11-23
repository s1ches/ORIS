using System.Net.Sockets;
using System.Net;
using MyProtocolServer.Protocol.XPacket;
using MyProtocolClient;
using MyProtocolServer.Protocol.XPacketsTypes;

namespace TCPClient
{
    internal class Program
    {
        private static int _handshakeMagic;

        private static void Main()
        {
            Console.Title = "XClient";
            Console.ForegroundColor = ConsoleColor.White;

            var client = new XClient();
            client.OnPacketRecieve += OnPacketRecieve;
            client.Connect("127.0.0.1", 2323);

            var rand = new Random();
            _handshakeMagic = rand.Next();

            Thread.Sleep(1000);

            Console.WriteLine("Sending handshake packet..");
           
            //RegisterTypes();

            client.QueuePacketSend(
                MyProtocolSerializator.Serialize(
                    XPacketType.Handshake,
                    new XPacketHandshake
                    {
                        MagicHandshakeNumber = _handshakeMagic
                    })
                    .ToPacket());

            while (true) 
            {
                
            }
        }

        private static void OnPacketRecieve(byte[] packet)
        {
            var parsed = XPacket.Parse(packet);

            if (parsed is not null)
                ProcessIncomingPacket(parsed);       
        }

        private static void ProcessIncomingPacket(XPacket packet)
        {
            var type = XPacketTypeManager.GetTypeFromPacket(packet);

            switch (type)
            {
                case XPacketType.Handshake:
                    ProcessHandshake(packet);
                    break;
                case XPacketType.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void ProcessHandshake(XPacket packet)
        {
            var handshake = MyProtocolSerializator.Deserialize<XPacketHandshake>(packet);

            if (_handshakeMagic - handshake.MagicHandshakeNumber == 15)
                Console.WriteLine("Handshake successful!");
        }

        private static void RegisterTypes()
        {
            XPacketTypeManager.RegisterType(XPacketType.Handshake, 0,0);
        }
    }
}