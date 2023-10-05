using System.Net;
using System.Reflection;

namespace MyHTTPServer.handlers;

public class ControllerHandler : Handler
{
    private readonly Assembly _controllerAssembly;
    public static Dictionary<string, Action<HttpListenerContext>> Actions { get; private set; }

    public ControllerHandler()
    {
        _controllerAssembly = Assembly.GetEntryAssembly();
        
        Actions = _controllerAssembly.GetTypes()
            .Where(x => typeof(Handler).IsAssignableFrom(x))
            .SelectMany(Handler => Handler.GetMethods()
                .Select(Method => new { Handler, Method }))
            .ToDictionary(
                key => GetPath(key.Handler, key.Method),
                value => GetEndpointMethod(value.Handler, value.Method)
                );
    }
    
    public override void HandleRequest(HttpListenerContext context)
    {
        throw new NotImplementedException();
    }

    private string GetPath(Type handler, MethodInfo method)
    {
        throw new NotImplementedException();
    }

    private Action<HttpListenerContext> GetEndpointMethod(Type handler, MethodInfo method)
    {
        throw new NotImplementedException();
    }
}