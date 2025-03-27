using EMS.Enums;
using System.Text.Json.Serialization;

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

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LeaveTypeEnum LeaveType { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StatusEnum Status { get; set; } = StatusEnum.Pending;

        public string? Reason { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    }
}
