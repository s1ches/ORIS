using Npgsql;

namespace Theme8.Example1;

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

        var npsqlExpression = "SELECT count(*) FROM Clients";
        
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            var command = new NpgsqlCommand(npsqlExpression, connection);
            var clientsCount = command.ExecuteScalar();

            command.CommandText = "SELECT MIN(clientage) from Clients";
            var minAge = command.ExecuteScalar();

            Console.WriteLine("Кол-во клиентов: {0}", clientsCount);
            Console.WriteLine("Минимальный возраст: {0}", minAge);
        }
    }
}