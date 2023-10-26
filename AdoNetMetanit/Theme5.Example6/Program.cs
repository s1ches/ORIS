using Npgsql;

namespace Theme5.Example6;

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

        var clientStatus = (ClientStatus)Int32.Parse(GetAttribute("status"));
        var clientFullName = GetAttribute("full name");
        var clientAge = Int32.Parse(GetAttribute("age"));
        var clientContactDetails = GetAttribute("contact details");
        var isBlockedClient = Boolean.Parse(GetAttribute("is blocked"));
        var isAnonymousClient = Boolean.Parse(GetAttribute("is anonymous"));;
        
        
        var npsqlExpression = String.Format("INSERT INTO Clients" +
                                            "(status, fullname, clientage, contactdetails, isblocked, isanonymous)" +
                                            "values ({0}, '{1}', {2}, '{3}', {4}, {5})", 
            (int)clientStatus, clientFullName, clientAge, clientContactDetails, isBlockedClient, isAnonymousClient);

        
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            
            var command = new NpgsqlCommand(npsqlExpression, connection);
            int number = command.ExecuteNonQuery();
            Console.WriteLine("Добавлено: {0} объектов", number);
            
            clientFullName = GetAttribute("new full name");
            
            npsqlExpression = String.Format("UPDATE Clients SET fullname='{0}' where clientage=19", clientFullName);
            command.CommandText = npsqlExpression;
            number = command.ExecuteNonQuery();
            Console.WriteLine("Обновлено: {0} объектов", number);
        }

        static string GetAttribute(string fieldName) 
        {
            Console.Write("Enter client {0}: ", fieldName);
            return Console.ReadLine();
        }
    }
}