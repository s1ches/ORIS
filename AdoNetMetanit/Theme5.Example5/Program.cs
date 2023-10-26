using Npgsql;

namespace Theme5.Example5;

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
        
        var npsqlExpression = $@"DELETE FROM Clients 
                                WHERE fullname='Сосорин Иван Сергеевич'";
        
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            var command = new NpgsqlCommand(npsqlExpression, connection);
            int number = command.ExecuteNonQuery();
            Console.WriteLine("Удалено: {0} объектов", number);
        }
        
    }
}