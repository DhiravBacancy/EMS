using EMS.Enums;
using EMS.Models;
using EMS.Helpers;
using System.ComponentModel.DataAnnotations;

namespace EMS.DTOs
{
    public class AddLeaveDTO
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [EndTimeAfterStartTimeHelper]
        public DateTime EndDate { get; set; }

        [Required]
        public LeaveTypeEnum LeaveType { get; set; }

        [MaxLength(255)]
        public string Reason { get; set; }

        public StatusEnum Status { get; set; } = StatusEnum.Pending;
    }

    public class UpdateLeaveDTO
    {   
        public DateTime StartDate { get; set; }

        [EndTimeAfterStartTimeHelper]
        public DateTime? EndDate { get; set; }

        public LeaveTypeEnum? LeaveType { get; set; }

        [MaxLength(255)]
        public string? Reason { get; set; }

        public StatusEnum? Status { get; set; } = StatusEnum.Pending;

        public DateTime? AppliedAt { get; set; } = DateTime.UtcNow;

    }

    public class LeaveApprovalDTO
    {
        [Required]
        public int LeaveId { get; set; }

        [Required]
        public StatusEnum Status { get; set; }
    }

    public class LeaveRequestDTO
    {
        public string EmployeeName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; } // If Status is required
    }

}
