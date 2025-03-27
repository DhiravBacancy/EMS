using EMS.Enums;
using System;
using System.Text.Json.Serialization;

namespace EMS.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
        public int? DepartmentId { get; set; }
        public Department Department { get; set; }
        public ICollection<Leave>? Leaves { get; set; }
        public ICollection<TimeSheet>? TimeSheets { get; set; }
        public string? TechStack { get; set; }
        public int PaidLeavesRemaining { get; set; } = 18;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Use System.Text.Json's JsonStringEnumConverter
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RolesEnum Role { get; set; } = RolesEnum.Employee;
    }
}
