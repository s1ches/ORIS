namespace MyORM_TEST.Models;

public enum ClientStatus{
    Standart,
    VIP,
    Platinum
}

public enum Gender
{
    Female,
    Male
}

public class Clients
{
    public Int64 id { get; set; }
    public Int16 status { get; set; } = default(Int32);
    public string fullname { get; set; } = String.Empty;
    public Int32 clientage { get; set; } = default(Int32);
    public string contactdetails { get; set; } = String.Empty;
    public bool? isblocked { get; set; } = default(bool);
    public bool? isanonymous { get; set; } = default(bool);
    public Int16? gender { get; set; } = default(Int16);
}