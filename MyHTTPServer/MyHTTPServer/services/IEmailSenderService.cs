using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHTTPServer.services
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string city, string address, string name,
            string surname, string birthday, string phoneNumber, string email);
    }
}
