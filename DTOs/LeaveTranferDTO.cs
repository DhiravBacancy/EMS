using EMS.Enums;

namespace EMS.DTOs
{
    public class LeaveTransferDTO
    {
        public int LeaveId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public LeaveTypeEnum LeaveType { get; set; }
        public string? Reason { get; set; }
        public StatusEnum Status { get; set; }
        public DateTime AppliedAt { get; set; }
    }
}
