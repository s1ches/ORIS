using MimeKit;
using MyHTTPServer.configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyHTTPServer
{
    public class StaticFilesHandler : IHandler
    {
        private static Dictionary<string, string> MimeTypes;
        private static readonly AppSettingConfig _serverConfig = ServerConfig.GetConfig();

        public StaticFilesHandler()
        {
            MimeTypes = new Dictionary<string, string>() 
            {
                {"html", "text/html"},
                {"/",  "text/html"},
                {"css" , "text/css"},
                {"jpg" , "image/jpeg"},
                {"svg", "image/svg+xml"},
                {"png", "image/png"},
                {"ico", "image/x-icon"}
            };
        }

        public async Task Handle(HttpListenerContext context)
        {
            var response = context.Response;
            var request = context.Request;
            var requestUrl = string.Join("", request.RawUrl!.Skip(1).ToArray());
            
            var path = $@"../../../{_serverConfig.StaticFilesPath}/index.html";
            var staticFilePath = $@"../../../{_serverConfig.StaticFilesPath}/{requestUrl}";
            
            var isFind = false;

            byte[] responseBuffer;

            if ((requestUrl.StartsWith("imgs") || requestUrl.StartsWith("styles")) && File.Exists(staticFilePath))
            {
                isFind = true;
                responseBuffer = File.ReadAllBytes(staticFilePath);
            }
            else if (File.Exists(path))
            {
                isFind = true;
                responseBuffer = File.ReadAllBytes(path);
            }
            else responseBuffer = Encoding.Default.GetBytes("<h4>404 - Not Found</h4>");

            response.StatusCode = isFind ? (int)HttpStatusCode.OK : (int)HttpStatusCode.NotFound;

            if (responseBuffer.Length > 0)
            {
                response.ContentType = GetContentType(request);
                response.ContentLength64 = responseBuffer.Length;
                using Stream output = response.OutputStream;
                await output.WriteAsync(responseBuffer);
                await output.FlushAsync();
                output.Close();
            }

            response.Close();
            await Console.Out.WriteLineAsync($"Запрос обработан");
        }

        private static string GetContentType(HttpListenerRequest request) => MimeTypes[request.RawUrl.Split(".").Last()]; 
    }
}
