using EMS.Enums;
using EMS.Helpers;
using System.ComponentModel.DataAnnotations;

namespace EMS.DTOs
{
    public class AddAdminDTO
    {
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }

        [Required]
        [ValidEmailHelper("@gmail.com")]
        public string Email { get; set; }

        [Required]
        public string Passsword { get; set; }

        [ValidPhoneNumberHelper]
        public string Phone { get; set; }

    }

    public class UpdateAdminDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [ValidEmailHelper("@gmail.com")]
        public string? Email { get; set; }

        [ValidPhoneNumberHelper]
        public string? Phone { get; set; }
    }

}
