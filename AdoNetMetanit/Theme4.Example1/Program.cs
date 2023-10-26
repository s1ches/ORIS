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

        var connection = new NpgsqlConnection(connectionString);
        
        connection.Open();
        Console.WriteLine(connection.ProcessID);
        connection.Close();
        
        connection.Open();
        Console.WriteLine(connection.ProcessID);
        connection.Close();
    }
}