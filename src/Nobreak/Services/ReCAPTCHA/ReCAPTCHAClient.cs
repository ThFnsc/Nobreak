using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
        
        private readonly IConfiguration _configuration;

        public string SiteKey =>
            _configuration["Variables:RecaptchaSiteKey"];

        public ReCAPTCHAClient(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
        }

        public virtual async Task<bool> Passed(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;
            var res = await _httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={_configuration["Variables:RecaptchaSecret"]}&response={token}");
            if (res.StatusCode != HttpStatusCode.OK)
                return false;
            var obj = JsonConvert.DeserializeObject<dynamic>(await res.Content.ReadAsStringAsync());
            return obj.success == "true" && obj.action == "login";
        }
    }
}
