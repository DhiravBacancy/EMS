using System.ComponentModel.DataAnnotations;
using EMS.Helpers;

namespace EMS.DTOs
{
    public class VerifyOTPDTO
    {
        [Required]
        [ValidEmailHelper]
        public string Email { get; set; }
        public string EnteredOTP { get; set; }

        public string NewPassword { get; set; }
    }
}
