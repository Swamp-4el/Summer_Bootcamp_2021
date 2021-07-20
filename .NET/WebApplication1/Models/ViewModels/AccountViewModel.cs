using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.ViewModels
{
    public class AccountViewModel
    {
        [Required(ErrorMessage = "Enter login")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Enter email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public bool Visible { get; set; }

        public int Identifier { get; set; }
    }
}
