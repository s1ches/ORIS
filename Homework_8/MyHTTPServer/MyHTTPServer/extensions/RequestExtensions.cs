using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MyHTTPServer.extensions;

public static class RequestExtensions
{
    public static object[] GetQueryParams(this HttpListenerRequest request, MethodInfo method)
    {
        var strParams = ParseRequest(request).Result;
        var strparams = request.QueryString;
        
        var queryParams = Array.Empty<object>();
        
        if (strparams.Count > 0)
        {
            queryParams = method.GetParameters()
                .Select((p, i) => Convert.ChangeType(strparams.GetValues(i).FirstOrDefault(), p.ParameterType))
                .ToArray();
        }
        
        if (strParams.Length > 0)
        {
            queryParams = method.GetParameters()
                .Select((p, i) => Convert.ChangeType(strParams[i].Split($": ").Last(), p.ParameterType))
                .ToArray();
        }

        return queryParams;
    }
    
    private static async Task<string[]> ParseRequest(this HttpListenerRequest request)
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