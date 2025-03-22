using EMS.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.DTOs
{
    public class AddTimeSheetDTO
    {
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }

    public class UpdateTimeSheetDTO
    {
        
        public TimeSpan StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
