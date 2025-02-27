using System.ComponentModel.DataAnnotations;

namespace MyApp.Models
{
    public class UsersModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public String Password { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
