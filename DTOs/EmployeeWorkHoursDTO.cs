namespace EMS.DTOs
{
    public class EmployeeWorkHoursDTO
    {
        public DateTime Date { get; set; }
        public int HoursWorked { get; set; }
    }
    public class EmployeeDetailsDTO
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Department { get; set; }
        public string TechStack { get; set; }
        public List<TimeSheetDto> TimeSheets { get; set; }
    }

}

