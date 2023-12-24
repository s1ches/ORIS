using MyHTTPServer;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var server = new Server();
        await server.StartServer();
    }
}