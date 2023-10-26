using Npgsql;

namespace Theme5.Example3;

enum ClientStatus{
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
        
        var npsqlExpression = @$"INSERT INTO Clients(status, fullname, clientage, contactdetails, isblocked, isanonymous)
                                    values ({(int)ClientStatus.VIP}, 'Сосорин Иван Сергеевич', 19, '1chessmic@gmail.com', False, False)";
        
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            var command = new NpgsqlCommand(npsqlExpression, connection);
            int number = command.ExecuteNonQuery();
            Console.WriteLine("Добавлено: {0} объектов", number);
        }
        
    }
}