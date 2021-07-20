using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Enter login!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Enter email!")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter password!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords don't match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
