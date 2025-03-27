using EMS.Helpers;
using System.ComponentModel.DataAnnotations;

namespace EMS.DTOs
{
    public class LoginDTO
    {
        [Required]
        [ValidEmailHelper]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
