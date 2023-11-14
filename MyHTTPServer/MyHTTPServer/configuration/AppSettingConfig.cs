using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyHTTPServer.configuration
{
    public class AppSettingConfig
    {
        public string? StaticFilesPath { get; set; }
        public uint Port { get; set; }
        public string? Address { get; set; }
        public string? FromName { get; set; }
        public string? EmailSender { get; set; }
        public string? PasswordSender { get; set; }
        public string? SMTPServerHost { get; set; }
        public ushort SMTPServerPort { get; set; }
        public string? ConnectionString { get; set; }

        private static readonly Lazy<AppSettingConfig> Lazy = new(() =>
        {
            var configPath = @"appsetting.json";
            if (!File.Exists(configPath))
                throw new Exception();

            using var jsonConfig = File.OpenRead(configPath);
            return JsonSerializer.Deserialize<AppSettingConfig>(jsonConfig)!;
        });

        public static AppSettingConfig Instance => Lazy.Value;
    }
}
