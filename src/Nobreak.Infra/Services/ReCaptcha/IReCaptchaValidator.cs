using System.Threading.Tasks;

namespace Nobreak.Infra.Services.ReCaptcha
{
    public interface IReCaptchaValidator
    {
        Task<bool> Passed(string token);
        string SiteKey { get; }
        bool Ready { get; }
    }
}
