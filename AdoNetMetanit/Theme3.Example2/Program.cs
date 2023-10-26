using Npgsql;

namespace Theme3.Example2;

class Program
{
    static void Main(string[] args)
    {
        var connectionString = @"Server=localhost;
                                    Port=5432;
                                    Database=StripClub;
                                    Username=postgres;
                                    Password=postgres";

        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            Console.WriteLine("Подключение открыто");
        }
        
        Console.WriteLine("Подключение закрыто");
    }
}