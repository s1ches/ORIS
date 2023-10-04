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
        public AppSettingConfig configInfo { get; }

        public ServerConfig(string configPath)
        {
            if (!File.Exists(configPath))
                throw new Exception();

            using (var jsonConfig = File.OpenRead(configPath))
            {
                configInfo = JsonSerializer.Deserialize<AppSettingConfig>(jsonConfig);
            }
        }
    }
}
