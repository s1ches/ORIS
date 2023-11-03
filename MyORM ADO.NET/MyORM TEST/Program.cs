using MyORM;
using MyORM_TEST.Configuration;
using MyORM_TEST.Models;

class Program
{
    static void Main()
    {
        var db = new MyDataContext(Settings.GetAppSettings().ConnectionString);
        
        var client = new Clients()
        {
            id = 2,
            fullname = "Коснырев Лев Сергеевич",
            clientage = 19,
            gender = 1,
            status = 2,
            contactdetails = @"iamquantum4@gmail.com",
            isanonymous = false,
            isblocked = false
        };

        //db.Add(client);

        //db.Delete<Clients>(6);

        //Console.WriteLine(db.SelectById<Clients>(2)?.fullname);

        //db.Update(client);

        //Console.WriteLine(db.SelectById<Clients>(2)?.fullname);
    }
}