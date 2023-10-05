using System.Net;
using System.Text.RegularExpressions;
using MyHTTPServer.configuration;

namespace MyHTTPServer.handlers;

[HttpController("FormController")]
public class FormController
{
    async public void SendForm(HttpListenerContext context)
    {
        var emailSender = new EmailSenderService();
        var toEmail = "1chessmic@gmail.com";
        var subject = "hw";
        var message = CreateEmailFormMessage(context.Request).Result;
        await emailSender.SendEmailAsync(toEmail, subject, message);
        context.Response.Redirect($"{ServerConfig.GetConfig().Address}:{ServerConfig.GetConfig().Port}/");
        context.Response.Close();
        await Task.Delay(1000);
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