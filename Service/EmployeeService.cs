using EMS.Data;
using EMS.DTOs;
using EMS.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.Service
{
    public interface IEmployeeService
    {
        Task<EmployeeDetailsDTO> GetEmployeeDetailsAsync(int employeeId, string reportType);
        Task<EmployeeDashboardDTO> GetEmployeeDashboardDataAsync(int  employeeId);

        Task<AdminDashboardDTO> GetAdminDashboardDataAsync(int employeeId);
        //Task<TimesheetDTO> GetEmployeeActivites(int employeeId);
    }


    public class EmployeeService : IEmployeeService
    {
        private readonly IGenericDBService<Employee> _employeeService;
        private readonly ApplicationDbContext _context;

        public EmployeeService(IGenericDBService<Employee> employeeService, ApplicationDbContext applicationDbContext)
        {
            _employeeService = employeeService;
            _context = applicationDbContext;
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
              .Select(ts => new TimesheetDTO
              {
                  Date = ts.Date.ToString(),
                  HoursWorked = (double)ts.TotalHours.GetValueOrDefault() // Convert decimal? to double
              })
              .ToList() ?? new List<TimesheetDTO>();


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


        public async Task<EmployeeDashboardDTO> GetEmployeeDashboardDataAsync(int employeeId)
        {
            var employee = await _context.Employees
                .Where(e => e.EmployeeId == employeeId)
                .Include(e => e.Leaves)
                .Include(e => e.TimeSheets)
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                return null; // Employee not found, return null or handle appropriately.
            }

            // Convert to list and process calculations in memory
            var timeSheets = employee.TimeSheets?.OrderByDescending(ts => ts.Date).Take(5).ToList() ?? new List<TimeSheet>();

            var totalLoggedHours = timeSheets
                .Where(ts => ts.EndTime.HasValue) // Ensure EndTime exists
                .Sum(ts => (double)(ts.TotalHours ?? 0)); // Explicit conversion from decimal? to double

            var latestTimesheets = timeSheets.Select(ts => new TimesheetDTO
            {
                Date = ts.Date.ToShortDateString(),
                HoursWorked = (double)(ts.TotalHours ?? 0) // Explicit conversion from decimal? to double
            }).ToList();

            return new EmployeeDashboardDTO
            {
                TotalLoggedHours = (int)Math.Round(totalLoggedHours),
                //LeaveBalance = CalculateLeaveBalance(employee.Leaves),
                LatestTimesheets = latestTimesheets
            };
        }

        public async Task<AdminDashboardDTO> GetAdminDashboardDataAsync(int employeeId)
        {
            var employee = await _context.Employees
                .Include(e => e.Leaves)
                .Include(e => e.TimeSheets)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null)
            {
                return null; // Employee not found, return null or handle appropriately.
            }

            double totalLoggedHours = employee.TimeSheets?
                .Where(ts => ts.EndTime.HasValue)
                .Sum(ts => (double)(ts.TotalHours ?? 0)) ?? 0;

            var topTimesheets = employee.TimeSheets?
                .OrderByDescending(ts => ts.Date)
                .Take(5)
                .Select(ts => new TimesheetDTO
                {
                    Date = ts.Date.ToString("yyyy-MM-dd"),
                    HoursWorked = (double)(ts.TotalHours ?? 0)
                })
                .ToList() ?? new List<TimesheetDTO>();

            var pendingLeaves = employee.Leaves?
                .Where(l => l.Status == Enums.StatusEnum.Pending)
                .OrderBy(l => l.StartDate)
                .Take(5)
                .Select(l => new LeaveRequestDTO
                {
                    EmployeeName = $"{l.Employee.FirstName} {l.Employee.LastName}",
                    StartDate = l.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = l.EndDate.ToString("yyyy-MM-dd") ?? "N/A",
                    Reason = l.Reason,
                    Status = l.Status.ToString()
                })
                .ToList() ?? new List<LeaveRequestDTO>();

            return new AdminDashboardDTO
            {
                TotalEmployees = 1, // Only 1 employee will be returned
                TotalLoggedHours = (int)Math.Round(totalLoggedHours),
                TopEmployees = new List<EmployeeActivityDTO>
        {
            new EmployeeActivityDTO
            {
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                TotalHoursWorked = (int)Math.Round(totalLoggedHours),
                EmployeeEmail = employee.Email,
                //Department = employee.Department, // Include the department here
                //Role = employee.Role // Include the role here
            }
        },
                PendingLeaveRequests = pendingLeaves,
                EmployeeEmail = employee.Email
            };
        }



        //public async Task<> GetEmployeeActivites(int employeeId)
        //{
        //    var existingEmployee = (await _employeeService.GetByMultipleConditionsAsync(
        //        new List<FilterDTO>
        //        {
        //            new FilterDTO { PropertyName = "EmployeeId", Value = employeeId }
        //        }
        //    )).FirstOrDefault();

        //    if (existingEmployee != null)
        //    {

        //    }
        //    return new TimeSheetDto
        //    {

        //    };
        //}
    }

}
