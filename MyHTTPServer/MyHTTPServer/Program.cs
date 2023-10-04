using MyHTTPServer;
using System.IO;
using System.IO.Enumeration;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

class Program
{
    private static async Task Main(string[] args)
    {
        var server = new Server();
        await server.Start();
    }
}