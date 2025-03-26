using System.ComponentModel.DataAnnotations;
using EMS.Helpers;

namespace EMS.DTOs
{
    public class VerifyOTPDTO
    {
        [Required]
        [ValidEmailHelper]
        public string Email { get; set; }

        [Required]
        public string EnteredOTP { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
