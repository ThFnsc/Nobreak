namespace Nobreak.Infra.Services.ReCaptcha
{
    public interface IReCaptchaRequired
    {
        public string ReCaptchaToken { get; set; }
    }
}
