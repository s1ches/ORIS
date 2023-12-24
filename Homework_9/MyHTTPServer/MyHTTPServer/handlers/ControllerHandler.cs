using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using MyHTTPServer.configuration;
using MyHTTPServer.extensions;

namespace MyHTTPServer.handlers;

public class ControllerHandler : Handler
{
    private static readonly AppSettingConfig ServerConfig;

    static ControllerHandler()
    {
        ServerConfig = AppSettingConfig.Instance;
    }
    
    public override async Task HandleRequest(HttpListenerContext context)
    {
        var response = context.Response;
        var request = context.Request;
        
        var assembly = Assembly.GetEntryAssembly();
        var controller = context.GetController(assembly);
        if (controller is null)
        {
            await HandleMethodResult(context, "not found controller");
            return;
        }
        
        var method = context.GetMethod(controller);
        if (method is null) 
        {
            await HandleMethodResult(context, "not found method");
            return;
        }
        
        var needAuh = method.GetCustomAttributes()
            .Select(attr => attr.GetType().Name)
            .Contains("NeedAuhAttribute", StringComparer.OrdinalIgnoreCase);
        
        var hasUserCookie = request.Cookies["id"] is not null;
        if(needAuh && !hasUserCookie)
            response.Redirect($"{ServerConfig.Address}:{ServerConfig.Port}/login.html");


        object[] queryParams;
        try
        {
            queryParams = request.GetQueryParams(method);
        }
        catch (Exception ex)
        {
            await HandleMethodResult(context, ex.Message);
            return;
        }

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
            response.Redirect($"{ServerConfig.Address}:{ServerConfig.Port}/");
        
        response.Close();
    }
}