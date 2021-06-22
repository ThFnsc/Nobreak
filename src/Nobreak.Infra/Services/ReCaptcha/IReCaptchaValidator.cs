using System.Threading.Tasks;

namespace Nobreak.Infra.Services.ReCaptcha
{
    public interface IReCaptchaValidator
    {
        Task<bool> PassedAsync(string token, string action);
        string SiteKey { get; }
        bool Ready { get; }
    }
}
