using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nobreak.Services.ReCAPTCHA
{
    public class ReCaptchaAllowAll : ReCAPTCHAClient
    {
        public ReCaptchaAllowAll(IConfiguration configuration) : base(configuration) { }

        public override async Task<bool> Passed(string token) =>
            await Task.Run(() => true);
    }
}
