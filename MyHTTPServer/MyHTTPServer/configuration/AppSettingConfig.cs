using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHTTPServer.configuration
{
    public class AppSettingConfig
    {
        public string StaticFilesPath { get; set; }
        public uint Port { get; set; }
        public string Address { get; set; }
        public string FromName { get; set; }
        public string EmailSender { get; set; }
        public string PasswordSender { get; set; }
        public string SMTPServerHost { get; set; }
        public ushort SMTPServerPort { get; set; }

    }
}
