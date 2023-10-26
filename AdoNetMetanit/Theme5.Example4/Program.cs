using Npgsql;

namespace Theme5.Example4;

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
        
       var npsqlExpression = $@"UPDATE Clients SET clientage=19
                                where fullname='Коснырев Лев Сергеевич'";
        
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            var command = new NpgsqlCommand(npsqlExpression, connection);
            int number = command.ExecuteNonQuery();
            Console.WriteLine("Обновлено: {0} объектов", number);
        }
        
    }
}