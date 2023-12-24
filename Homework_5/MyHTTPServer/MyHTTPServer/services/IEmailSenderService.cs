namespace MyHTTPServer.services;

public interface IEmailSenderService
{
    Task SendEmailAsync(string city, string address, string name,
        string surname, string birthday, string phoneNumber, string email);
}

