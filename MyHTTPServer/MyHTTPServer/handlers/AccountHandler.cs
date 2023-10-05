using System.Net;
using System.Text.RegularExpressions;

namespace MyHTTPServer.handlers;

public class AccountHandler : Handler
{
    async public override void HandleRequest(HttpListenerContext context)
    {
        var emailSender = new EmailSenderService();
        var toEmail = "1chessmic@gmail.com";
        var subject = "hw";
        var message = CreateEmailFormMessage(context.Request).Result;
        await emailSender.SendEmailAsync(toEmail, subject, message);
    }
    
    private async Task<string> CreateEmailFormMessage(HttpListenerRequest request)
    {
        if (!request.HasEntityBody)
            return default(string);

        var stream = new StreamReader(request.InputStream);
        var str = await stream.ReadToEndAsync();
        str = Uri.UnescapeDataString(Regex.Unescape(str));
        str = str.Replace("&", "\n");
        str = str.Replace("=", ": ");
        str = str.Replace("+", " ");

        return str;
    }
}