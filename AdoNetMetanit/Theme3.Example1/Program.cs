using Npgsql;

namespace Theme3.Example1;

class Program
{
    static void Main(string[] args)
    {
        var connectionString = @"Server=localhost;
                                    Port=5432;
                                    Database=StripClub;
                                    Username=postgres;
                                    Password=postgres";

        var connection = new NpgsqlConnection(connectionString);
        
        try
        {
            connection.Open();
            Console.WriteLine("Подключение открыто");
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
            Console.WriteLine("Подключение закрыто");
        }
    }
}