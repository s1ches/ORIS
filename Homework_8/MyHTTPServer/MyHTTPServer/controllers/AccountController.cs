using MyHTTPServer.attributes;
using MyHTTPServer.configuration;
using MyHTTPServer.model;
using MyHTTPServer.services;
using MyORM;

namespace MyHTTPServer.controllers;

[HttpController("AccountController")]
public class AccountController
{
    [Post("SendEmail")]
    public async void SendEmail(string city, string address, string name,
        string surname, string birthday, string phoneNumber, string email)
    { 
        await new EmailSenderService().SendEmailAsync(city, address, name, surname, birthday,
            phoneNumber, email);
    }

   [Get("SendForm2")]
    public string SendForm2()
    {
        var htmlCode = "<html><head></head><body>Hi SendForm2</body></html>";
        return htmlCode;
    }
    
    [Get("GetAll")]
    [NeedAuh]
    public Account[] GetAll()
    {
        var db = new MyDataContext(AppSettingConfig.Instance.ConnectionString);

        var accounts = db.Select<Account>().ToArray();

        return accounts;
    }

    [Post("Add")]
    public void Add(string email, string password)
    {
        var db = new MyDataContext(AppSettingConfig.Instance.ConnectionString);
        var newAccount = new Account()
        {
            email = email,
            password = password
        };

        db.Add(newAccount);
    }
    
    [Post("Delete")]
    public void Delete(int id)
    {
        var db = new MyDataContext(AppSettingConfig.Instance.ConnectionString);
        if(db.SelectById<Account>(id) is not null)
            db.Delete<Account>(id);
    }
    
    [Post("Update")]
    public void Update(int id, string email, string password)
    {
        var db = new MyDataContext(AppSettingConfig.Instance.ConnectionString);

        if (db.SelectById<Account>(id) is null)
            return;
        
        var account = new Account()
        {
            id = id,
            email = email,
            password = password
        };
        
        db.Update(account);
    }
    
    [Get("GetById")]
    [NeedAuh]
    public Account? GetById(int id)
    {
        var db = new MyDataContext(AppSettingConfig.Instance.ConnectionString);

        var account = db.SelectById<Account>(id);

        return account;
    }
}