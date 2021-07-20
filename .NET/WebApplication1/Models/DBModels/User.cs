using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.DBModels
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int Identifier { get; set; }

        public bool Visible { get; set; }

        public int Key { get; set; }
    }
}
