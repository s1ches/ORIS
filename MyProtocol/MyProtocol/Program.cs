using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProtocolServer
{
    internal class Program
    {
        private static void Main()
        { 
            var server = new XServer();
            server.Start();
            server.AcceptClients();
        }

    }
}
