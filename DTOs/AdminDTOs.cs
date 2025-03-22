using EMS.Enums;

namespace EMS.DTOs
{
    public class AddAdminDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public RolesEnum Role { get; set; } = RolesEnum.Admin;
    }

    public class UpdateAdminDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
