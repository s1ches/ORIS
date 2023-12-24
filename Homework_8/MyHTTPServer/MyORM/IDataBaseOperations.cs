namespace MyORM;

public interface IDataBaseOperations
{
    public bool Add<T>(T? obj);
    public bool Update<T>(T? obj);
    public bool Delete<T>(int id);
    public List<T> Select<T>();
    public T? SelectById<T>(int id);
}