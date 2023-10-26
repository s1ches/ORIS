using Npgsql;

namespace Theme7.Example1;

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

            for(int i=0;i<7;i++)
                Console.Write("{0}{1}", reader.GetName(i), i==6 ? "\n" : "\t");
            
            if (!reader.HasRows)
                throw new ArgumentException();

            while (reader.Read()){
              var  id = reader.GetInt32(0);
              var contact = reader.GetString(4);
              Console.WriteLine("{0}\t{1}", id, contact);
            }

            reader.Close();
        }
    }
}