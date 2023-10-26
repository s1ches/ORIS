using Npgsql;

namespace Theme4.Example1;

class Program
{
    static void Main(string[] args)
    {
        var connectionString = @"Server=localhost;
                                    Port=5432;
                                    Database=StripClub;
                                    Username=postgres;
                                    Password=postgres";
        
        var connectionString2 = @"Server=localhost;
                                    Port=5432;
                                    Database=lectures;
                                    Username=postgres;
                                    Password=postgres";

        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            Console.WriteLine(connection.ProcessID);
        }
        
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            Console.WriteLine(connection.ProcessID);
        }
        
        using (var connection = new NpgsqlConnection(connectionString2))
        {
            connection.Open();
            Console.WriteLine(connection.ProcessID);
        }
    }
}