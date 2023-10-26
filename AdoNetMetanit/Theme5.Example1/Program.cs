using Npgsql;

namespace Theme5.Example1;

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
            var command = new NpgsqlCommand();
            command.CommandText = "SELECT * FROM Clients";
            command.Connection = connection;
        }
        
    }
}