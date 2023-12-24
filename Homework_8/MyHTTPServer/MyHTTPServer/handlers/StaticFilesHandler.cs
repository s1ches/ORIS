using System.Net;
using System.Text;
using MyHTTPServer.configuration;

namespace MyHTTPServer.handlers;

public class StaticFilesHandler : Handler
{
    private static readonly Dictionary<string, string> MimeTypes = new (){            
        {"html", "text/html"},
        {"/",  "text/html"},
        {"css" , "text/css"},
        {"jpg" , "image/jpeg"},
        {"svg", "image/svg+xml"},
        {"png", "image/png"},
        {"ico", "image/x-icon"}
    };
    
    private static readonly AppSettingConfig _serverConfig;

    static StaticFilesHandler()
    {
        _serverConfig = AppSettingConfig.Instance;
    }
    
    public override async Task HandleRequest(HttpListenerContext context)
    {
        if (IsStaticFilesRequested(context.Request)) await GiveStaticFileResponse(context);
        else if(Successor != null) await Successor.HandleRequest(context);
    }
    
    private static string GetContentType(HttpListenerRequest request) => MimeTypes[request.RawUrl!.Split(".").Last()];
    private static bool IsStaticFilesRequested(HttpListenerRequest request) => MimeTypes.Keys.Contains(request.RawUrl!.Split(".").Last());
    private async Task GiveStaticFileResponse(HttpListenerContext context)
    {
        var response = context.Response;
        var request = context.Request;
        var requestUrl = string.Join("", request.RawUrl!.Skip(1).ToArray());
            
        var path = $@"../../../{_serverConfig.StaticFilesPath}/index.html";
        var staticFilePath = $@"../../../{_serverConfig.StaticFilesPath}/{requestUrl}";
        var isFind = false;

        byte[] responseBuffer;

        if ((requestUrl.StartsWith("imgs") || requestUrl.StartsWith("styles") || requestUrl.EndsWith("html")) && File.Exists(staticFilePath))
        {
            isFind = true;
            responseBuffer = await File.ReadAllBytesAsync(staticFilePath);
        }
        else if (File.Exists(path))
        {
            isFind = true;
            responseBuffer = await File.ReadAllBytesAsync(path);
        }
        else responseBuffer = Encoding.Default.GetBytes("<h4>404 - Not Found</h4>");

        response.StatusCode = isFind ? (int)HttpStatusCode.OK : (int)HttpStatusCode.NotFound;

        if (responseBuffer.Length > 0)
        {
            response.ContentType = GetContentType(request);
            response.ContentLength64 = responseBuffer.Length;
            await using Stream output = response.OutputStream;
            await output.WriteAsync(responseBuffer);
            await output.FlushAsync();
            output.Close();
        }

        response.Close();
    }
}