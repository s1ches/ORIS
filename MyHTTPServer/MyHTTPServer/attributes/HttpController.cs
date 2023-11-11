namespace MyHTTPServer.attributes;

public class HttpController : Attribute
{
    public string ControllerName { get; set; }
    public HttpController(string controllerName)
    {
        ControllerName = controllerName;
    }
}