using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyHTTPServer.configuration
{
    public class ServerConfig
    {
        private static AppSettingConfig? ConfigInfo { get; set; }
        private static readonly string configPath = @"configuration/appsetting.json";

        private ServerConfig()
        {
            if (!File.Exists(configPath))
                throw new Exception();

            using (var jsonConfig = File.OpenRead(configPath))
            {
                ConfigInfo = JsonSerializer.Deserialize<AppSettingConfig>(jsonConfig);
            }
        }

        public static AppSettingConfig GetConfig()
        {
            if (ConfigInfo == null)
                new ServerConfig();

            return ConfigInfo;
        }
    }
}
