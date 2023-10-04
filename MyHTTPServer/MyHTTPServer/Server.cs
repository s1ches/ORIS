using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MyHTTPServer.configuration;

namespace MyHTTPServer
{
    public class Server
    {
        private HttpListener _listener;
        private static AppSettingConfig _serverConfig;
        private static bool _isStop = true; 
        private static CancellationTokenSource _cts;

        public Server()
        {
            _serverConfig = ServerConfig.GetConfig();
            _listener = new HttpListener();
            _listener.Prefixes.Add($"{_serverConfig.Address}:{_serverConfig.Port}/");
            _cts = new CancellationTokenSource();
        }

        public async Task Start()
        {
            _isStop = false;
            var serverListeningTask = Task.Run( async () => await StartServerListening(_cts.Token), _cts.Token);
            while (!_isStop)
                _isStop = await IsStop();

            _cts.Cancel();

            if (_listener.IsListening)
            {
                await Console.Out.WriteLineAsync("Сервер остановлен");
                _listener.Stop();
            }
            await Console.Out.WriteLineAsync("Работа сервера завершена");
        }

        async private Task StartServerListening(CancellationToken _token)
        {
            if (_listener.Prefixes.Count == 0)
                throw new ArgumentException("Server has no prefixes");

            try
            {  
                _listener.Start();
                await Console.Out.WriteLineAsync("Сервер запущен");
                var staticFilesHandler = new StaticFilesHandler();
                var emailSender = new EmailSenderService();

                while (!_token.IsCancellationRequested)
                {
                    var context = await _listener.GetContextAsync();
                    
                    if (context.Request.HttpMethod.ToLower().Equals("get"))
                        await staticFilesHandler.Handle(context);
                    else if (context.Request.HttpMethod.ToLower().Equals("post"))
                    {
                        string message = CreateEmailFormMessage(context.Request).Result;
                        await emailSender.SendEmailAsync("1chessmic@gmail.com", "hw", message);
                        //await emailSender.SendEmailAsync("t.mukhutdinov.job@gmail.com", "HW", message);
                        context.Response.Redirect(_listener.Prefixes.First());
                        context.Response.Close();
                        await Task.Delay(1000);
                    }
                }
            }
            catch (Exception ex) { /*await Console.Out.WriteLineAsync(ex.Message);*/ }
        }

        private async Task<bool> IsStop() =>  Console.ReadLine().ToLower().Equals("stop");

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
}
