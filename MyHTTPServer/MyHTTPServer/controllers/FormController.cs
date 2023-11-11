using MyHTTPServer.attributes;
using MyHTTPServer.model;
using MyHTTPServer.services;

namespace MyHTTPServer.controllers;

[HttpController("FormController")]
public class FormController
{
    [Post("SendForm")]
    public async void SendForm(string city, string address, string name,
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
    
    [Get("GetAccounts")]
    [NeedAuh]
    public Account[] GetAccounts()
    {
        var accounts = new Account[]
        {
            new() { email = "email-1", password = "pass-1" },
            new() { email = "email-2", password = "pass-2" }
        };
        
        return accounts;
    }
}