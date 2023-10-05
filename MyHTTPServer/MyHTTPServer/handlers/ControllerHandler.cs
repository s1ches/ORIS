using System.Net;
using System.Reflection;

namespace MyHTTPServer.handlers;

public class ControllerHandler : Handler
{
    private Assembly _controllerAssembly;
    
    public override void HandleRequest(HttpListenerContext context)
    {
        var strParams = context.Request.Url
            .Segments
            .Skip(1)
            .Select(s => s.Replace("/", ""))
            .ToArray();

        var controllerName = strParams[0];

        _controllerAssembly = Assembly.GetEntryAssembly();
        
        var controller = _controllerAssembly.GetTypes()
            .Where(t => Attribute.IsDefined(t, typeof(HttpController)))
            .FirstOrDefault(c => (((HttpController)Attribute.GetCustomAttribute(c, typeof(HttpController))!)!)
                .ControllerName.Equals(controllerName+"controller", StringComparison.OrdinalIgnoreCase));
        
        if(controller==null) throw new Exception("null controller");
        
        var methodName = strParams[1];
        
        var method = controller.GetMethods()
            .FirstOrDefault(t => t.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));

        if (method == null) throw new Exception("null method");
        
        var ret = method.Invoke(Activator.CreateInstance(controller), new object[] {context});
    }
}