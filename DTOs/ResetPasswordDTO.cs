using System.ComponentModel.DataAnnotations;
using EMS.Helpers;
namespace EMS.DTOs
{
    public class ResetPasswordDTO
    {
        [Required]
        [ValidEmailHelper]
        public string Email { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
