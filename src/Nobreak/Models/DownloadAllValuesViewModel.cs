using Nobreak.Infra.Services.ReCaptcha;

namespace Nobreak.Models
{
    public class DownloadAllValuesViewModel : IReCaptchaRequired
    {
        public string ReCaptchaToken { get; set; }
    }
}
