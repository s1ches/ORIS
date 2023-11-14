using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using MyHTTPServer.configuration;
using MyHTTPServer.extensions;

namespace MyHTTPServer.handlers;

public class ControllerHandler : Handler
{
    private static readonly AppSettingConfig? _serverConfig;

    static ControllerHandler()
    {
        _serverConfig = ServerConfig.GetConfig();
    }
    
    public override async Task HandleRequest(HttpListenerContext context)
    {
        var response = context.Response;
        var request = context.Request;
        
        var assembly = Assembly.GetEntryAssembly();
        var controller = context.GetController(assembly);
        if(controller is null) throw new ArgumentException("null controller");
        
        var method = context.GetMethod(controller);
        if (method is null) throw new ArgumentException("null method");
        
        var needAuh = method.GetCustomAttributes()
            .Select(attr => attr.GetType().Name)
            .Contains("NeedAuhAttribute", StringComparer.OrdinalIgnoreCase);
        
        var hasUserCookie = request.Cookies["id"] is not null;
        if(needAuh && !hasUserCookie)
            response.Redirect($"{_serverConfig.Address}:{_serverConfig.Port}/login.html");

        var queryParams = request.GetQueryParams(method);
        
        var methodResult = method.Invoke(Activator.CreateInstance(controller), queryParams);
        await HandleMethodResult(context, methodResult);
    }

    private async Task HandleMethodResult(HttpListenerContext context, object? methodResult)
    {
        var response = context.Response;
        var request = context.Request;
        
        byte[] responseBuffer = Array.Empty<byte>();
        
        if (methodResult is string resultString)
            responseBuffer = Encoding.UTF8.GetBytes(resultString);
        else if (methodResult is Cookie cookie)
            response.Cookies.Add(cookie);
        else if (methodResult is not null)
        {
            var serializeObj  = JsonSerializer.Serialize(methodResult);
            responseBuffer = Encoding.UTF8.GetBytes(serializeObj);
        }

        if (responseBuffer.Length > 0)
        {
            response.ContentLength64 = responseBuffer.Length;
            await using Stream output = response.OutputStream;
            await output.WriteAsync(responseBuffer);
            await output.FlushAsync();
            output.Close();
        }
        
        if(request.HttpMethod.Equals("post", StringComparison.OrdinalIgnoreCase))
            response.Redirect($"{_serverConfig.Address}:{_serverConfig.Port}/");
        
        response.Close();
    }
}