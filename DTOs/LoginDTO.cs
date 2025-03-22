using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace EMS.DTOs
{
    public class LoginDTO
    {
        public string Email { get; set; }
        public String Password { get; set; }
    }
}
