using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nobreak.Infra
{
    public class AppSettings
    {
        public string SerialPort { get; set; }

        public string RecaptchaSiteKey { get; set; }

        public string RecaptchaSecret { get; set; }

        public int? BauldRate { get; set; }
    }
}
