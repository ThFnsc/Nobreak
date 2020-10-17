using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nobreak.Infra.Services.ReCaptcha
{
    public interface IReCaptchaRequired
    {
        public string ReCaptchaToken { get; set; }
    }
}
