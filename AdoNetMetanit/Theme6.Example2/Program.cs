using Npgsql;

namespace Theme6.Example2;

public enum ClientStatus{
    VIP,
    STANDART,
    PLATINUM
}

class Program
{
    static void Main(string[] args)
    {
        var connectionString = @"Server=localhost;
                                    Port=5432;
                                    Database=StripClub;
                                    Username=postgres;
                                    Password=postgres";

        var npsqlExpression = "SELECT * FROM Clients";
        
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            var command = new NpgsqlCommand(npsqlExpression, connection);
            
            var reader = command.ExecuteReader();

            if (!reader.HasRows)
                throw new ArgumentException();
            
            while (reader.Read())
                Console.WriteLine("{0}\t",reader["contactdetails"]);  
            
            
            reader.Close();
        }
    }
}