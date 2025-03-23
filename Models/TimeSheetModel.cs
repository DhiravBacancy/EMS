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
        public TimeSpan? EndTime { get; set; }

        [NotMapped] // Ensures this is NOT stored in the database
        public decimal? TotalHours => EndTime.HasValue ? (decimal)(EndTime.Value - StartTime).TotalHours : null;

        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
