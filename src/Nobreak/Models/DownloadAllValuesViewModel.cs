﻿using Nobreak.Infra.Services.ReCaptcha;
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