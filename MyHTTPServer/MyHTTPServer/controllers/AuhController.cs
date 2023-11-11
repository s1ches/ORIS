using System.Net;
using MyHTTPServer.attributes;
using MyHTTPServer.configuration;

namespace MyHTTPServer.controllers;

[HttpController("AuhController")]
public class AuhController
{
    [Post("Login")]
    public Cookie Login(string login, string password)
    {
        var config = ServerConfig.GetConfig();
        return new Cookie("id", GetCustomerId(login, password))
        {
            Domain = config.Address.Split("//")[1],
            Path = "/",
            Expires = DateTime.Today.AddMonths(1)
        };
    }
    
    private string GetCustomerId(string login, string password) => login+password.GetHashCode();
    
}