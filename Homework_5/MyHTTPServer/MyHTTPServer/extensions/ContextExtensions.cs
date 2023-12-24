using System.Net;
using System.Reflection;
using MyHTTPServer.attributes;

namespace MyHTTPServer.extensions;

public static class ContextExtensions
{
    private static string[] GetParams(this HttpListenerContext context)
    {
        return  context.Request.Url!
            .Segments
            .Skip(1)
            .Select(s => s.Replace("/", ""))
            .ToArray();
    }
    private static string GetControllerName(this HttpListenerContext context) => GetParams(context)[0];
    private static string GetMethodName(this HttpListenerContext context) => GetParams(context)[1];

    public static Type? GetController(this HttpListenerContext context, Assembly? assembly)
    {
        var controllerName = context.GetControllerName();
        
        return assembly!.GetTypes()
            .Where(type => Attribute.IsDefined(type, typeof(HttpController)))
            .FirstOrDefault(type => ((HttpController)Attribute.GetCustomAttribute(type, typeof(HttpController))!)
                .ControllerName.Equals(controllerName+"controller", StringComparison.OrdinalIgnoreCase));
    }

    public static MethodInfo? GetMethod(this HttpListenerContext context, Type controller)
    {
        return controller.GetMethods()
            .Where(x => x.GetCustomAttributes(true)
                .Any(attr => attr.GetType().Name.Equals($"{context.Request.HttpMethod}Attribute",
                    StringComparison.OrdinalIgnoreCase)))
                .FirstOrDefault(m => m.Name.Equals(context.GetMethodName(), StringComparison.OrdinalIgnoreCase));
    }
}