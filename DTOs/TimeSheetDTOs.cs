using EMS.Helpers;
using System.ComponentModel.DataAnnotations;

namespace EMS.DTOs
{
    public class AddTimeSheetDTO
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [EndTimeAfterStartTimeHelper]
        public TimeSpan EndTime { get; set; }
        public String Description { get; set; }
    }
    public class UpdateTimeSheetDTO
    {
        public TimeSpan? StartTime { get; set; }

        [EndTimeAfterStartTimeHelper]
        public TimeSpan? EndTime { get; set; }

        public string Description { get; set; }
    }

    public class TimesheetDTO
    {
        public string Date { get; set; }
        public double HoursWorked { get; set; }
    }


}
