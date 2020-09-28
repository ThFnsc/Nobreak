using Nobreak.Services.ReCAPTCHA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nobreak.Models
{
    public class DownloadAllValuesViewModel : IReCaptchaRequired
    {
        public string ReCaptchaToken { get; set; }
    }
}
