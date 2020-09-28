using Nobreak.Services.ReCAPTCHA;
using System.ComponentModel.DataAnnotations;

namespace Nobreak.Models
{
    public class LoginModel : IReCaptchaRequired
    {
        [Required(ErrorMessage = "O campo é obrigatório")]
        [Display(Name = "E-mail")]
        [EmailAddress(ErrorMessage = "Não é um e-mail válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo é obrigatório")]
        [Display(Name = "Senha")]
        public string Password { get; set; }
        
        public string  ReCaptchaToken { get; set; }
    }
}