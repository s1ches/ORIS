﻿using MailKit.Net.Smtp;
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
        private static readonly string _pathToZip = @"../../../zip.zip";
        private static readonly AppSettingConfig _serverConfig = ServerConfig.GetConfig();

        public async Task SendEmailAsync(string toEmail, string subject, string message)
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

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_serverConfig.SMTPServerHost, _serverConfig.SMTPServerPort, true);
                await client.AuthenticateAsync(_serverConfig.EmailSender, _serverConfig.PasswordSender);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);       
            }
            
            await Console.Out.WriteLineAsync($"Письмо отправлено по адресу {toEmail}");
        }
    }
}

