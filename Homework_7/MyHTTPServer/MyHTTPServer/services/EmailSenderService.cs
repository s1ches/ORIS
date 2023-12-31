﻿using MimeKit;
using MyHTTPServer.configuration;
using System.Net.Mail;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace MyHTTPServer.services;

public class EmailSenderService : IEmailSenderService
{
    private static readonly string _pathToZip = @"../../../zip.zip";
    private static readonly AppSettingConfig _serverConfig = AppSettingConfig.Instance;

    public async Task SendEmailAsync(string city, string address, string name,
        string surname, string birthday, string phoneNumber, string email)
    {
        var message = $"{city}\n{address}\n{name}\n{name}\n{surname}\n{birthday}\n{phoneNumber}\n{email}";
        Console.WriteLine(email);
        await DoSendEmailAsync(email.Split(" ").Last(), "Dodo HR", message);
    }
    
    private async Task DoSendEmailAsync(string toEmail, string subject, string message)
    {
        using var emailMessage = new MimeMessage();
        
        emailMessage.From.Add(new MailboxAddress(_serverConfig.FromName, _serverConfig.EmailSender));
        emailMessage.To.Add(new MailboxAddress("", toEmail));

        emailMessage.Subject = subject;
        var body = new TextPart(MimeKit.Text.TextFormat.Html){ Text = message };

        var attachment = new MimePart("application/zip")
        {
            Content = new MimeContent(File.OpenRead(_pathToZip)),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = Path.GetFileName("HWzip")
        };

        var multipart = new Multipart("mixed");
        multipart.Add(body);
        multipart.Add(attachment);

        emailMessage.Body = multipart;

        try
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_serverConfig.SMTPServerHost, _serverConfig.SMTPServerPort, true);
                await client.AuthenticateAsync(_serverConfig.EmailSender, _serverConfig.PasswordSender);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
            Console.WriteLine($"Письмо отправлено по адресу {toEmail}");
        }
        catch (SmtpException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}


