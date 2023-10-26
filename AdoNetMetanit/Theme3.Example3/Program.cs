using Npgsql;

namespace Theme3.Example3;

class Program
{
    static async Task Main(string[] args)
    {
        await ConnectWithDB();
    }

    private static async Task ConnectWithDB()
    {
        var connectionString = @"Server=localhost;
                                Port=5432;
                                Database=StripClub;
                                Username=postgres;
                                Password=postgres";
        
        using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();
            Console.WriteLine("Подключение открыто");
        }
        
        Console.WriteLine("Подключение закрыто");
    }
    
}