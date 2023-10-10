using System.Net;
using System.Reflection.Metadata;
using System.Text;
using MyHTTPServer.configuration;

namespace MyHTTPServer.handlers;

public class StaticFilesHandler : Handler
{
    private static Dictionary<string, string> MimeTypes = new Dictionary<string, string>(){            
        {"html", "text/html"},
        {"/",  "text/html"},
        {"css" , "text/css"},
        {"jpg" , "image/jpeg"},
        {"svg", "image/svg+xml"},
        {"png", "image/png"},
        {"ico", "image/x-icon"}
    };
    private static readonly AppSettingConfig _serverConfig = ServerConfig.GetConfig();
    
    async public override void HandleRequest(HttpListenerContext context)
    {
        if (IsStaticFilesRequested(context.Request)) await GiveStaticFileResponse(context);
        else if(Successor != null) Successor.HandleRequest(context);
    }
    
    private static string GetContentType(HttpListenerRequest request) => MimeTypes[request.RawUrl!.Split(".").Last()];
    private static bool IsStaticFilesRequested(HttpListenerRequest request) => MimeTypes.Keys.Contains(request.RawUrl!.Split(".").Last());

    async private Task GiveStaticFileResponse(HttpListenerContext context)
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
    }
}