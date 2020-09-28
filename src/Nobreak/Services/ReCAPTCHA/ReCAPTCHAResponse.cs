using System;
using System.Text.Json.Serialization;

namespace Nobreak.Services.ReCAPTCHA
{
    internal class ReCAPTCHAResponse
    {
        public bool Success { get; set; }
        public float Score { get; set; }
        public string Action { get; set; }
        public DateTime Challenge_ts { get; set; }
        public string Hostname { get; set; }
        [JsonPropertyName("error-codes")]
        public string[] ErrorCodes { get; set; }

        internal bool Ok(string action) =>
            Success && Action == action;
    }
}