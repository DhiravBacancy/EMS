using System.ComponentModel.DataAnnotations;

namespace EMS.DTOs
{
    public class EmployeeWorkReportRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
         public string Period { get; set; } = "monthly";
    }


    public class EmployeeWorkReportDTO
    {
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public int TotalHoursWorked { get; set; }
        public int LeavesTaken { get; set; }
        public int PaidLeavesRemaining { get; set; }
        public List<TimeSheetDTOForReport> TimeSheets { get; set; }
    }
    public class TimeSheetDTOForReport
    {
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public decimal? TotalHours { get; set; }
        public string Description { get; set; }
    }


}
