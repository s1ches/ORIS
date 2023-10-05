using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MyHTTPServer.configuration;
using MyHTTPServer.handlers;

namespace MyHTTPServer
{
    public class Server
    {
        private HttpListener _listener;
        private static AppSettingConfig _serverConfig;
        private static bool _isStop = true; 
        private CancellationTokenSource _cts;

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
                
                Handler staticFilesHandler = new StaticFilesHandler();
                Handler controllerHandler = new ControllerHandler();
                staticFilesHandler.Successor = controllerHandler;

                while (!_token.IsCancellationRequested)
                {
                    var context = await _listener.GetContextAsync();
                    staticFilesHandler.HandleRequest(context);
                }
            }
            catch (Exception ex) { /*await Console.Out.WriteLineAsync(ex.Message);*/ }
        }

        private async Task<bool> IsStop() =>  Console.ReadLine().ToLower().Equals("stop");
    }
}
