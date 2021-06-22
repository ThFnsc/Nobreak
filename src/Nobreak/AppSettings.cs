namespace Nobreak
{
    public class AppSettings
    {
        public string SerialPort { get; set; }

        public string RecaptchaSiteKey { get; set; }

        public string RecaptchaSecret { get; set; }

        public int? BauldRate { get; set; }

        public bool RunMigrationsOnStartup { get; set; }

        public bool RequireLoginToDownloadValues { get; set; }
    }
}
