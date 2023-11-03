using System.Data;
using System.Reflection;
using System.Text;
using Npgsql;

namespace MyORM;

public class MyDataContext : IDataBaseOperations
{
    private NpgsqlConnection? _connection;
    private readonly string? _connectionString;

    public MyDataContext(string? connectionString)
    {
        if (connectionString is null) throw new NullReferenceException("connection string is null");
        
        _connectionString = connectionString; 
    }
    
    public bool Add<T>(T? obj)
    {
        if (obj == null) throw new ArgumentNullException();
        
        var tableName = obj.GetType().Name;

        var tableFields = obj.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => !property.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var arguments = new NpgsqlParameter[tableFields.Count];
        
        var commandBuilder = new StringBuilder();
        commandBuilder.AppendFormat("INSERT INTO {0}(", tableName);
        
        foreach (var field in tableFields)
            commandBuilder.AppendFormat("{0},", field.Name);
        
        commandBuilder.Length--;
        commandBuilder.Append(") values (");
        
        var argumentNumber=0;
        foreach (var field in tableFields)
        {
            var parameterName = String.Format(@"@{0}", field.Name);
            commandBuilder.AppendFormat("{0},", parameterName);
            arguments[argumentNumber++] = new NpgsqlParameter(parameterName, field.GetValue(obj));
        }

        commandBuilder.Length--;
        commandBuilder.Append(");");
        
        var command = new NpgsqlCommand(commandBuilder.ToString());
        command.Parameters.AddRange(arguments);

        return ExecuteNonQueryCommand(command, "Insert");
    }

    public bool Update<T>(T? obj)
    {
        if (obj is null) throw new NullReferenceException();
        
        var tableName = obj.GetType().Name;
        var tableFields = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToList();
        
        var commandBuilder = new StringBuilder();
        commandBuilder.AppendFormat("UPDATE {0} SET", tableName);
        
        var arguments = new NpgsqlParameter[tableFields.Count];

        var argumentNumber=0;
        foreach (var field in tableFields)
        {
            var parameterName = String.Format("@{0}", field.Name);
            arguments[argumentNumber++]  = new NpgsqlParameter(parameterName, field.GetValue(obj));
            if(!field.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                commandBuilder.AppendFormat(" {0} = {1},", field.Name, parameterName);
        }

        commandBuilder.Length--;
        commandBuilder.Append(" WHERE id = @id;");
        
        var command = new NpgsqlCommand(commandBuilder.ToString());
        command.Parameters.AddRange(arguments);
        
        return ExecuteNonQueryCommand(command, "Update");
    }

    public bool Delete<T>(int id)
    {
        var tableName = typeof(T).Name;

        var npsqlExpression = String.Format("DELETE FROM {0} WHERE id = @id", tableName);
        
        var idParam = new NpgsqlParameter("@id", id);
        var command = new NpgsqlCommand(npsqlExpression);
        command.Parameters.Add(idParam);

        return ExecuteNonQueryCommand(command, "Delete");
    }
    
    public List<T> Select<T>()
    {
        var tableName = typeof(T).Name;
        var npsqlExpression = String.Format("SELECT * FROM {0}", tableName);
        var command = new NpgsqlCommand(npsqlExpression, _connection);

        return ExecuteQueryCommand<T>(command);
    }

    public T? SelectById<T>(int id)
    {
        var tableName = typeof(T).Name;

        var npsqlExpression = String.Format("SELECT * FROM {0} WHERE id = @id", tableName);
        
        var idParam = new NpgsqlParameter("@id", id);
        var command = new NpgsqlCommand(npsqlExpression, _connection);
        command.Parameters.Add(idParam);

        return ExecuteQueryCommand<T>(command).FirstOrDefault();
    }
    
    private bool ExecuteNonQueryCommand(NpgsqlCommand command, string commandType)
    {
        var result = 0;
        try
        {
            using (_connection = new NpgsqlConnection(_connectionString))
            {
                _connection.Open();
                command.Connection = _connection;
                result = command.ExecuteNonQuery();
                Console.WriteLine("{0} {1} object", commandType, result);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return result > 0;
    }

    private List<T> ExecuteQueryCommand<T>(NpgsqlCommand command)
    {
        var result = new List<T>();
        var tableFields = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToList();
        
        try
        {
            using (_connection = new NpgsqlConnection(_connectionString))
            {
                _connection.Open();
                command.Connection = _connection;
                var adapter = new NpgsqlDataAdapter(command);

                var dataSet = new DataSet();
                adapter.Fill(dataSet);
                
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    var relustItem = Activator.CreateInstance<T>();
                    var tableFieldNumber = 0;
                    
                    foreach (DataColumn column in dataSet.Tables[0].Columns)
                    {
                        var value = row[column] is not DBNull ? row[column] : null; 
                        tableFields[tableFieldNumber++].SetValue(relustItem, value);
                    }
                    result.Add(relustItem);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return result;
    }
}