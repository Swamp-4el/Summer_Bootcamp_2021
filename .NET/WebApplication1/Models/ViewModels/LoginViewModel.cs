using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Write user login")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Write password!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
