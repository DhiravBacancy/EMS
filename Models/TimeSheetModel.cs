
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Models
{
    public class TimeSheet
    {
        public int TimesheetId { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        [Column(TypeName = "DECIMAL(5,2)")]
        public decimal TotalHours { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
