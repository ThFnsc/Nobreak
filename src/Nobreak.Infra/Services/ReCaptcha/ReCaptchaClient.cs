using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Nobreak.Infra.Services.ReCaptcha
{
    public class ReCaptchaClient : IReCaptchaValidator
    {
        private readonly HttpClient _httpClient;
        private readonly string _secret;
        private readonly ILogger<ReCaptchaClient> _logger;

        public string SiteKey { get; }

        public bool Ready =>
            !string.IsNullOrWhiteSpace(SiteKey) && !string.IsNullOrWhiteSpace(_secret);

        public ReCaptchaClient(HttpClient httpClient, string siteKey, string secret, ILogger<ReCaptchaClient> logger)
        {
            _httpClient = httpClient;
            SiteKey = siteKey;
            _secret = secret;
            _logger = logger;
        }

        public async Task<bool> PassedAsync(string token, string action)
        {
            if (!Ready)
            {
                _logger.LogWarning("ReCAPTCHA not configured. Letting through");
                return true;
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Empty token");
                return false;
            }

            var uriBuilder = new UriBuilder(_httpClient.BaseAddress);
            var qs = HttpUtility.ParseQueryString(uriBuilder.Query);
            qs["secret"] = _secret;
            qs["response"] = token;
            uriBuilder.Query = qs.ToString();

            var req = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            var res = await _httpClient.SendAsync(req);

            res.EnsureSuccessStatusCode();

            var resString = await res.Content.ReadAsStringAsync();
            var parsed = JsonSerializer.Deserialize<ReCaptchaResponse>(resString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (!parsed.Ok(action))
            {
                _logger.LogWarning("ReCAPTCHA Fail: {Model}\nToken: {Token}", res.ToJson(), token);
                return false;
            }
            return true;
        }
    }
}
