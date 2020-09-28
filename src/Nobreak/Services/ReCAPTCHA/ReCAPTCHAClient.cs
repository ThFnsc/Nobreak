using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nobreak.Services.ReCAPTCHA
{
    public class ReCaptchaClient : IReCaptchaValidator
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;
        private readonly ILogger<ReCaptchaClient> _logger;

        public string SiteKey =>
            _appSettings.RecaptchaSiteKey;

        public bool Ready =>
            !string.IsNullOrWhiteSpace(_appSettings.RecaptchaSiteKey) && !string.IsNullOrWhiteSpace(_appSettings.RecaptchaSecret);

        public ReCaptchaClient(HttpClient httpClient, IOptions<AppSettings> appSettings, ILogger<ReCaptchaClient> logger)
        {
            _httpClient = httpClient;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        public async Task<bool> Passed(string token)
        {
            if (!Ready)
            {
                _logger.LogWarning("ReCaptcha não configurado. Deixando passar");
                return true;
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Token vazio");
                return false;
            }

            var res = await _httpClient.MakeRequest(new HttpRequest<ReCAPTCHAResponse>
            {
                Method = HttpMethod.Get,
                Path = "siteverify",
                QueryString = new
                {
                    secret = _appSettings.RecaptchaSecret,
                    response = token
                }.ConvertToDictionary()
            });

            if (!res.Ok("login"))
            {
                _logger.LogWarning("Não passou no ReCaptcha: {Model}\nToken: {Token}", res.ToJson(), token);
                return false;
            }

            _logger.LogDebug("ReCaptcha Ok: {Token}", token);
            return true;
        }
    }
}
