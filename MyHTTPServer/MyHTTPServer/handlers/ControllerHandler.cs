using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using MyHTTPServer.configuration;
using Org.BouncyCastle.Asn1.Ocsp;

namespace MyHTTPServer.handlers;

public class ControllerHandler : Handler
{
    private Assembly? _controllerAssembly;
    
    async public override void HandleRequest(HttpListenerContext context)
    {
        var strParams = context.Request.Url!
            .Segments
            .Skip(1)
            .Select(s => s.Replace("/", ""))
            .ToArray();

        var controllerName = strParams[0];

        _controllerAssembly = Assembly.GetEntryAssembly();
        
        var controller = _controllerAssembly!.GetTypes()
            .Where(t => Attribute.IsDefined(t, typeof(HttpController)))
            .FirstOrDefault(c => (((HttpController)Attribute.GetCustomAttribute(c, typeof(HttpController))!)!)
                .ControllerName.Equals(controllerName+"controller", StringComparison.OrdinalIgnoreCase));
        
        if(controller==null) throw new ArgumentException("null controller");
        
        var methodName = strParams[1];
        
        var method = controller.GetMethods()
            .Where(x => x.GetCustomAttributes(true)
                .Any(attr => attr.GetType().Name.Equals($"{context.Request.HttpMethod}Attribute",
                    StringComparison.OrdinalIgnoreCase)))
            .FirstOrDefault(m => m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));

        if (method == null) throw new ArgumentException("null method");

        strParams = ParseRequest(context.Request).Result;
        
        var queryParams = Array.Empty<object>();
        
        if (strParams.Length > 0)
        {
            queryParams = method.GetParameters()
                .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                .ToArray();
        }
        
        var ret = method.Invoke(Activator.CreateInstance(controller), queryParams);
        byte[]? responseBuffer = Array.Empty<byte>();
        
        if (ret is string)
            responseBuffer = Encoding.UTF8.GetBytes((ret as string)!);
        else if (!(ret is null))
        {
            var serializeObj  = JsonSerializer.Serialize(ret);
            responseBuffer = Encoding.UTF8.GetBytes(serializeObj);
        }

        var response = context.Response;
        
        if (responseBuffer.Length > 0)
        {
            response.ContentLength64 = responseBuffer.Length;
            using Stream output = response.OutputStream;
            await output.WriteAsync(responseBuffer);
            await output.FlushAsync();
            output.Close();
        }

        if(context.Request.HttpMethod.Equals("post", StringComparison.OrdinalIgnoreCase))
        {
            var _serverConfig = ServerConfig.GetConfig();
            response.Redirect($"{_serverConfig.Address}:{_serverConfig.Port}/");
        }
        response.Close();
    }

    async private Task<string[]> ParseRequest(HttpListenerRequest request)
    {
        if (!request.HasEntityBody)
            return Array.Empty<string>();
        
        var stream = new StreamReader(request.InputStream);
        var requestData = await stream.ReadToEndAsync();
        requestData = Uri.UnescapeDataString(Regex.Unescape(requestData));
        requestData = requestData.Replace("&", "\n");
        requestData = requestData.Replace("=", ": ");
        requestData = requestData.Replace("+", " ");

        return requestData.Split("\n").ToArray();
    }
}