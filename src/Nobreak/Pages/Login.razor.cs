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
            [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Senha precisa ter no mínimo 8 caracteres e conter pelo menos 3 dessas 4 opções: letra maiúscula, letra minúscula, número e caractere especial")]
            public string Password { get; set; }
        }
    }
}
