using EMS.Helpers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace EMS.DTOs
{
    public class LoginDTO
    {
        [Required]
        [ValidEmailHelper]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
