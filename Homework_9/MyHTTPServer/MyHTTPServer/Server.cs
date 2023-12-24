using System.Net;
using MyHTTPServer.configuration;
using MyHTTPServer.handlers;

namespace MyHTTPServer
{
    public class Server : IDisposable
    {
        private readonly HttpListener _listener;
        private readonly CancellationTokenSource _cts;

        public Server()
        {
            var serverConfig = AppSettingConfig.Instance;
            _listener = new HttpListener();
            _listener.Prefixes.Add($"{serverConfig.Address}:{serverConfig.Port}/");
            _cts = new CancellationTokenSource();
        }

        public async Task StartServer()
        {
            if (_listener.Prefixes.Count == 0)
                throw new ArgumentException("Server has no prefixes");
            
            var token = _cts.Token; 
            var listenTask =  Task.Run(async () => await ListenAsync(token), token);

            await Task.Run(() =>
            {
                var isStop = false;
                while (!isStop && !_cts.IsCancellationRequested)
                    if(Console.KeyAvailable)
                        isStop = IsStop();
            }, token);
            
            _cts.Cancel();
            Task.WaitAny(listenTask);
        }

        private async Task ListenAsync(CancellationToken token)
        {
            try
            {
                _listener.Start();
                Console.WriteLine($"Сервер запущен {AppSettingConfig.Instance.Address}:{AppSettingConfig.Instance.Port}");

                Handler staticFilesHandler = new StaticFilesHandler();
                Handler controllerHandler = new ControllerHandler();
                staticFilesHandler.Successor = controllerHandler;
                
                var getContextTask = Task.Run(_listener.GetContextAsync, token);

                while (!token.IsCancellationRequested)
                {
                    if (!getContextTask.IsCompleted) continue;
                    
                    await staticFilesHandler.HandleRequest(getContextTask.Result);
                    Console.WriteLine("Запрос обработан");
                    getContextTask = Task.Run(_listener.GetContextAsync, token);
                }
                
                token.ThrowIfCancellationRequested();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally { Dispose(); }
        }

        private bool IsStop() => 
            string.Compare(Console.ReadLine(), "stop", StringComparison.OrdinalIgnoreCase) == 0;
        

        public void Dispose()
        {
            _listener.Stop();
            Console.WriteLine("Сервер остановлен");
            ((IDisposable)_listener).Dispose();
            Console.WriteLine("Работа сервера завершена");
        }
    }
}
