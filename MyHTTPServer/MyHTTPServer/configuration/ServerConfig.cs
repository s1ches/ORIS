using System.Text.Json;

namespace MyHTTPServer.configuration
{
    public class ServerConfig
    {
        private static AppSettingConfig? ConfigInfo { get; set; }
        private const string _configPath = @"configuration/appsetting.json";

        private ServerConfig()
        {
            if (!File.Exists(_configPath))
                throw new Exception();

            using var jsonConfig = File.OpenRead(_configPath);
            ConfigInfo = JsonSerializer.Deserialize<AppSettingConfig>(jsonConfig);
        }

        public static AppSettingConfig? GetConfig()
        {
            if (ConfigInfo == null)
                new ServerConfig();

            return ConfigInfo;
        }
    }
}
