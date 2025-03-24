using EMS.DTOs;
using EMS.Models;

namespace EMS.Service
{
    public interface IEmployeeService
    {
        Task<EmployeeDetailsDTO> GetEmployeeDetailsAsync(int employeeId, string reportType);
    }


    public class EmployeeService : IEmployeeService
    {
        private readonly IGenericDBService<Employee> _employeeService;

        public EmployeeService(IGenericDBService<Employee> employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<EmployeeDetailsDTO> GetEmployeeDetailsAsync(int employeeId, string reportType)
        {
            // Fetch the employee asynchronously
            var employee = (await _employeeService.GetAllAsync())
                .FirstOrDefault(e => e.EmployeeId == employeeId);

            if (employee == null)
            {
                throw new Exception("Employee not found.");
            }

            DateTime startDate, endDate;

            // Handle report type (weekly or monthly)
            if (reportType == "weekly")
            {
                startDate = DateTime.Now.AddDays(-7);
                endDate = DateTime.Now;
            }
            else if (reportType == "monthly")
            {
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                endDate = DateTime.Now;
            }
            else
            {
                throw new Exception("Invalid report type.");
            }

            // Ensure TimeSheets is not null before accessing it
            var timeSheets = employee.TimeSheets?
              .Where(ts => ts.Date >= startDate && ts.Date <= endDate)
              .OrderBy(ts => ts.Date)
              .Select(ts => new TimeSheetDto
              {
                  Date = ts.Date,
                  HoursWorked = (double)ts.TotalHours.GetValueOrDefault() // Convert decimal? to double
              })
              .ToList() ?? new List<TimeSheetDto>();


            return new EmployeeDetailsDTO
            {
                EmployeeId = employee.EmployeeId,
                FullName = $"{employee.FirstName} {employee.LastName}",
                Email = employee.Email,
                Phone = employee.Phone,
                Address = employee.Address,
                Department = employee.Department?.DepartmentName ?? "N/A", // Handle null Department
                TechStack = employee.TechStack,
                TimeSheets = timeSheets
            };
        }

    }


}
