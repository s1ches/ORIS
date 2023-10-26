using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace Theme11.Example1;

public enum ClientStatus{
    VIP,
    STANDART,
    PLATINUM
}

class Program
{
    private static string connectionString = @"Server=localhost;
                                    Port=5432;
                                    Database=StripClub;
                                    Username=postgres;
                                    Password=postgres";

    static void Main(string[] args)
    {
        // var status = Int32.Parse(Console.ReadLine()!);
        // var fullName = "'" + Console.ReadLine()! + "'";
        // var age = Int32.Parse(Console.ReadLine()!);
        // var contact = "'" + Console.ReadLine()! + "'";
        // var isBlocked = bool.Parse(Console.ReadLine()!);
        // var isAnonymous = bool.Parse(Console.ReadLine()!);
        //
        
        ShowClients();
        
    }

static void AddClient
        (int status, string fullName, int age, string contact, bool isBlocked, bool isAnonymous)
    {
        var npsqlExpression = "insertclient";

        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            var command = new NpgsqlCommand(npsqlExpression, connection);
            command.CommandType = CommandType.StoredProcedure;
            
            var statusParam = new NpgsqlParameter("_status", status);
            var fullNameParam = new NpgsqlParameter("_fullName", fullName);
            var ageParam = new NpgsqlParameter("_age", age);
            var contactParam = new NpgsqlParameter("_contact", contact);
            var isBlockedParam = new NpgsqlParameter("_isBlocked", isBlocked);
            var isAnonymousParam = new NpgsqlParameter("_isAnonymous", isAnonymous);
            
            command.Parameters.Add(statusParam);
            command.Parameters.Add(fullNameParam);
            command.Parameters.Add(ageParam);
            command.Parameters.Add(contactParam);
            command.Parameters.Add(isBlockedParam);
            command.Parameters.Add(isAnonymousParam);

            var result = command.ExecuteScalar();

            Console.WriteLine("id добавленного объекта: {0}", result);
        }
    }

    static void ShowClients()
    {
        var npsqlExpression = "public.getclients2";

        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            var command = new NpgsqlCommand(npsqlExpression, connection);
            
            command.CommandType = CommandType.StoredProcedure;
            
            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                Console.WriteLine("{0}\t{1}", reader.GetName(0), reader.GetName(1));
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var name = reader.GetString(1);
                    Console.WriteLine("{0}\t{1}", id, name);
                }
            }
            
            reader.Close();
        }
    }
}