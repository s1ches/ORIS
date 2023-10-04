using MailKit.Net.Smtp;
using MimeKit;
using MyHTTPServer.configuration;
using MyHTTPServer.services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MyHTTPServer
{
    public class EmailSenderService : IEmailSenderService
    {
        private static readonly string configPath = @"configuration/appsetting.json";
        private static readonly string pathToZip = @"../../../zip.zip";
        private ServerConfig serverConfig;

        public EmailSenderService()
        {
            serverConfig = new ServerConfig(configPath);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(serverConfig.configInfo.FromName, serverConfig.configInfo.EmailSender));
            emailMessage.To.Add(new MailboxAddress("", toEmail));

            emailMessage.Subject = subject;
            var body = new TextPart(MimeKit.Text.TextFormat.Html){ Text = message };

            var attachment = new MimePart("application/zip")
            {
                Content = new MimeContent(File.OpenRead(pathToZip)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName("HWzip")
            };

            var multipart = new Multipart("mixed");
            multipart.Add(body);
            multipart.Add(attachment);

            emailMessage.Body = multipart;

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(serverConfig.configInfo.SMTPServerHost, serverConfig.configInfo.SMTPServerPort, true);
                await client.AuthenticateAsync(serverConfig.configInfo.EmailSender, serverConfig.configInfo.PasswordSender);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);       
            }
            
            await Console.Out.WriteLineAsync($"Письмо отправлено по адресу {toEmail}");
        }
    }
}

