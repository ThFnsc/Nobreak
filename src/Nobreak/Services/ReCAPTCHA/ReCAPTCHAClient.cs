using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nobreak.Services.ReCAPTCHA
{
    public class ReCAPTCHAClient : IReCaptchaValidator
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;

        public string SiteKey =>
            _appSettings.RecaptchaSiteKey;

        public bool Ready =>
            !string.IsNullOrWhiteSpace(_appSettings.RecaptchaSiteKey) && !string.IsNullOrWhiteSpace(_appSettings.RecaptchaSecret);

        public ReCAPTCHAClient(HttpClient httpClient, IOptions<AppSettings> appSettings)
        {
            _httpClient = httpClient;
            _appSettings = appSettings.Value;
        }

        public async Task<bool> Passed(string token) =>
            !Ready || (await _httpClient.MakeRequest(new HttpRequest<ReCAPTCHAResponse>
            {
                Method = HttpMethod.Get,
                Path = "siteverify",
                QueryString = new
                {
                    secret = _appSettings.RecaptchaSecret,
                    response = token
                }.ConvertToDictionary()
            })).Ok("login");
    }
}
