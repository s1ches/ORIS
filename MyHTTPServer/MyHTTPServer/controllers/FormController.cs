using System.Net;
using System.Text.RegularExpressions;
using MyHTTPServer.configuration;
using MyHTTPServer.model;

namespace MyHTTPServer.handlers;

[HttpController("FormController")]
public class FormController
{
    [Post("SendForm")]
    async public void SendForm(string city, string address, string name,
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
    
    [Get("SendForm3")]
    public Account[] SendForm3()
    {
        var accounts = new Account[]
        {
            new Account() { email = "email-1", password = "pass-1" },
            new Account() { email = "email-2", password = "pass-2" }
        };

        return accounts;
    }
}