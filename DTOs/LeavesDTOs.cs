using EMS.Enums;
using EMS.Models;

namespace EMS.DTOs
{
    public class AddLeavesDTO
    {
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveTypeEnum LeaveType { get; set; }
        public string Reason { get; set; }
        public StatusEnum Status { get; set; } = StatusEnum.Pending;
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    }

    public class UdpateLeavesDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveTypeEnum LeaveType { get; set; }
        public string Reason { get; set; }
        public StatusEnum Status { get; set; } = StatusEnum.Pending;
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    }

    public class LeaveApprovalDTO
    {
        public int LeaveId { get; set; }
        public StatusEnum Status { get; set; }
    }
}
