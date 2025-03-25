using EMS.Enums;

namespace EMS.Models
{
    public class Leave
    {
        public int LeaveId { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int TotalDays { get; set; }

        public LeaveTypeEnum LeaveType { get; set; }

        public string? Reason { get; set; }

        public StatusEnum Status { get; set; } = StatusEnum.Pending;

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    }
}
