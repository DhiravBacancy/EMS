using EMS.Helpers;
using System.ComponentModel.DataAnnotations;

namespace EMS.DTOs
{


    // DTO for adding a new timesheet
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
    }

    // DTO for updating an existing timesheet
    public class UpdateTimeSheetDTO
    {
        public TimeSpan? StartTime { get; set; }

        [EndTimeAfterStartTimeHelper]
        public TimeSpan? EndTime { get; set; }
    }

    public class TimesheetDTO
    {
        public string Date { get; set; }
        public double HoursWorked { get; set; }
    }


}
