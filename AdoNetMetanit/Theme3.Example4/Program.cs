using Npgsql;

namespace Theme3.Example4;

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

            Console.WriteLine("Свойства подключения:");
            Console.WriteLine("\tСтрока подключения: {0}", connection.ConnectionString);
            Console.WriteLine("\tБаза данных: {0}", connection.Database);
            Console.WriteLine("\tСервер: {0}", connection.DataSource);
            Console.WriteLine("\tВерсия сервера: {0}", connection.ServerVersion);
            Console.WriteLine("\tСостояние: {0}", connection.State);
        }
        
        Console.WriteLine("Подключение закрыто");
    }
}