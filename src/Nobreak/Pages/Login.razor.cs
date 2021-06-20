using System.ComponentModel.DataAnnotations;

namespace Nobreak.Pages
{
    public partial class Login
    {
        public class LoginModel
        {
            [Display(Name = "E-mail")]
            [Required(ErrorMessage = "É necessário fornecer um e-mail")]
            [EmailAddress(ErrorMessage = "Precisa ser um endereço de e-mail válido")]
            public string Email { get; set; }

            [Display(Name = "Senha")]
            [Required(ErrorMessage = "É necessário fornecer uma senha")]
            [MinLength(8, ErrorMessage = "É necessário no mínimo 8 caracteres")]
            //[RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{8,})", ErrorMessage = "É necessário pelo menos uma letra minúscula, uma maiúscula e um número")]
            public string Password { get; set; }
        }
    }
}
