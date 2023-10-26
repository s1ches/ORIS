using Npgsql;

namespace Theme9.Example1;

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

        var npsqlExpression = @"INSERT INTO Client (status, fullname, clientage, contactdetails, isblocked, isanonymous) 
                                values (@status, @fullName , @age, @contact, @isBlocked, @isAnonymous)";

        var status = ClientStatus.STANDART;
        var fullName = "'Ивкин Сергей Викторович'";
        var age = 59;
        var contact = @"'ivkin_sport@mail.ru', False, False); INSERT INTO Client (status, fullname, clientage, contactdetails, isblocked, isanonymous) values (0, 'Hacker' , 101, 'hihi@mail.ru'";
        var isBlocked = false;
        var isAnonymous = false;
        
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            var command = new NpgsqlCommand(npsqlExpression, connection);

            var statusParam = new NpgsqlParameter("@status", (Int32)status);
            var fullNameParam = new NpgsqlParameter("@fullName", fullName);
            var ageParam = new NpgsqlParameter("@age", age);
            var contactParam = new NpgsqlParameter("@contact", contact);
            var isBlockedParam = new NpgsqlParameter("@isBlocked", isBlocked);
            var isAnonymousParam = new NpgsqlParameter("@isAnonymous", isAnonymous);

            command.Parameters.Add(statusParam);
            command.Parameters.Add(fullNameParam);
            command.Parameters.Add(ageParam);
            command.Parameters.Add(contactParam);
            command.Parameters.Add(isBlockedParam);
            command.Parameters.Add(isAnonymousParam);

            var number = command.ExecuteNonQuery();
            Console.WriteLine("Добавлено объектов: {0}", number);
        }
    }
}