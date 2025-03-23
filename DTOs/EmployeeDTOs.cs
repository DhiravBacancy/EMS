using EMS.Enums;
using EMS.Models;
using System.ComponentModel.DataAnnotations;
using EMS.Helpers;

namespace EMS.DTOs
{
    public class AddEmployeeDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [ValidEmailHelper("@gmail.com")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [ValidPhoneNumberHelper]
        public string Phone { get; set; }

        //custom validation
        [ValidDateOfBirthHelper(MinimumAge = 18)]
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
        public string? TechStack { get; set; }
    }

    public class UdpateEmployeeDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [ValidEmailHelper("@gmail.com")]
        public string? Email { get; set; }
        public string? Password { get; set; }
        [ValidPhoneNumberHelper]
        public string? Phone { get; set; }

        [ValidDateOfBirthHelper(MinimumAge = 18)]
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? TechStack { get; set; }
    }
}
