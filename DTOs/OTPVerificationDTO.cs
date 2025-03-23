using System.ComponentModel.DataAnnotations;
using EMS.Helpers;

namespace EMS.DTOs
{
    public class OTPVerificationDTO
    {
        [Required]
        [ValidEmailHelper]
        public string Email { get; set; }
        public string EnteredOTP { get; set; }
    }
}
