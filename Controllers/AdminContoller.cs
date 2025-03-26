using EMS.Service;
using Microsoft.AspNetCore.Mvc;
using EMS.DTOs;
using EMS.Models;
using EMS.Helpers;
using EMS.Services;
using EMS.Enums;


namespace EMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IGenericDBService<Admin> _adminService;
        private readonly IGenericDBService<TimeSheet> _timeSheetService;
        private readonly IGenericDBService<Employee> _employeeService;
        private readonly IGenericDBService<Leave> _leaveService;
        private readonly IGenericDBService<Department> _departmentService;
        private readonly IExportTimesheetsToExcelService _exportToExcelService;
        private readonly IPdfService _pdfService;


        public AdminController(IGenericDBService<Admin> adminService, 
                                IGenericDBService<TimeSheet> timeSheetService, 
                                IExportTimesheetsToExcelService exportTimesheetsToExcelService, 
                                IGenericDBService<Employee> employeeService, IGenericDBService<Leave> leaveService, 
                                IPdfService pdfService,
                                IGenericDBService<Department> departmentService)
        {
            _adminService = adminService;
            _timeSheetService = timeSheetService;
            _exportToExcelService = exportTimesheetsToExcelService;
            _employeeService = employeeService;
            _departmentService = departmentService;
            _leaveService = leaveService;
            _pdfService = pdfService;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddAdmin([FromBody] AddAdminDTO addAdminDto)
        {
            DTOValidationHelper.ValidateModelState(ModelState); // Throws validation error if invalid

            var existingAdmin = (await _adminService.GetByMultipleConditionsAsync(
                new List<FilterDTO> { new FilterDTO { PropertyName = "Email", Value = addAdminDto.Email } }
            )).FirstOrDefault();

            if (existingAdmin != null)
                throw new CustomException("Email is already in use", 400);

            var newAdmin = new Admin
            {
                FirstName = addAdminDto.FirstName,
                LastName = addAdminDto.LastName,
                Passsword = BCrypt.Net.BCrypt.HashPassword(addAdminDto.Passsword),
                Email = addAdminDto.Email,
                Phone = addAdminDto.Phone,
            };

            await _adminService.AddAsync(newAdmin);
            return Ok("Admin added successfully"); // ResponseWrapper will wrap this
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllAdmins()
        {
            var admins = await _adminService.GetAllAsync();
            if (admins == null || !admins.Any())
                throw new CustomException("No Admin Found", 404);

            return Ok(admins);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdminById(int id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            if (admin == null)
                throw new CustomException("Admin not found", 404);

            return Ok(admin);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateAdmin(int id, [FromBody] UpdateAdminDTO updateAdminDto)
        {
            DTOValidationHelper.ValidateModelState(ModelState);

            var existingAdmin = await _adminService.GetByIdAsync(id);
            if (existingAdmin == null)
                throw new CustomException("Admin not found", 404);

            var emailCheck = (await _adminService.GetByMultipleConditionsAsync(
                new List<FilterDTO> { new FilterDTO { PropertyName = "Email", Value = updateAdminDto.Email } }
            )).FirstOrDefault();

            if (emailCheck != null && emailCheck.AdminId != id)
                throw new CustomException("Email is already in use by another admin", 400);

            existingAdmin.FirstName = updateAdminDto.FirstName ?? existingAdmin.FirstName;
            existingAdmin.LastName = updateAdminDto.LastName ?? existingAdmin.LastName;
            existingAdmin.Email = updateAdminDto.Email ?? existingAdmin.Email;
            existingAdmin.Phone = updateAdminDto.Phone ?? existingAdmin.Phone;
            existingAdmin.UpdatedAt = DateTime.UtcNow;

            await _adminService.UpdateAsync(existingAdmin);
            return Ok("Admin updated successfully");
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            if (admin == null)
                throw new CustomException("Admin not found", 404);

            await _adminService.DeleteAsync(admin.AdminId);
            return Ok("Admin deleted successfully");
        }


        [HttpGet("getEmployeeDetail/{employeeEmail}")]
        public async Task<IActionResult> GetEmployeeDetail(string employeeEmail)
        {
            var employee = await _employeeService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "Email", Value = employeeEmail}
            });
            if (employee == null || !employee.Any())
                return BadRequest(new { Message = "No timesheets found for export." });
            return Ok(employee);
        }


        [HttpGet("getEmployeePendingLeaveRequest/{employeeEmail}")]
        public async Task<IActionResult> getEmployeesPendingLeaveRequest(string employeeEmail)
        {
            var employee = await _employeeService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "Email", Value = employeeEmail }
            });

            if (employee == null || !employee.Any())
            {
                return BadRequest(new { Message = "No Employee found for export." });
            }
            // Assuming your Employee object has an Id property (adjust if needed)
            var employeeId = employee.First().EmployeeId;

            var pendingLeaves = await _leaveService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "EmployeeId", Value = employeeId },
                new FilterDTO { PropertyName = "Status", Value = StatusEnum.Pending }
            });

            if (pendingLeaves == null || !pendingLeaves.Any())
            {
                return BadRequest(new { Message = "No timesheets found for export." });
            }

            var leaveDTOs = pendingLeaves.Select(leave => new LeaveTransferDTO
            {
                LeaveId = leave.LeaveId,
                StartDate = leave.StartDate,
                EndDate = leave.EndDate,
                TotalDays = leave.TotalDays,
                LeaveType = leave.LeaveType,
                Reason = leave.Reason,
                Status = leave.Status,
                AppliedAt = leave.AppliedAt
            }).ToList();

            return Ok(leaveDTOs);

        }


        [HttpGet("exportEmployeeTimeSheet/{employeeEmail}")]
        public async Task<IActionResult> ExportTimeSheets(string employeeEmail)
        {
            var employee = await _employeeService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "Email", Value = employeeEmail }
            });

            if (employee == null || !employee.Any())
            {
                return BadRequest(new { Message = "No Employee found for export." });
            }

            // Assuming your Employee object has an Id property (adjust if needed)
            var employeeId = employee.First().EmployeeId;

            var timeSheets = await _timeSheetService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "EmployeeId", Value = employeeId } // Use EmployeeId here
            });

            if (timeSheets == null || !timeSheets.Any())
            {
                return BadRequest(new { Message = "No timesheets found for export." });
            }

            // Convert IEnumerable<TimeSheet> to List<TimeSheet>
            var timeSheetList = timeSheets.ToList();

            Console.WriteLine($"Exporting {timeSheetList.Count} timesheets to Excel.");

            // Get the file content from the service
            var fileResult = await _exportToExcelService.ExportToExcel(timeSheetList);

            // Return the file directly (FileContentResult will handle it)
            return fileResult;
        }


        [HttpPost("generateEmployeeWorkReport")]
        public async Task<IActionResult> GenerateEmployeeWorkReport([FromBody] EmployeeWorkReportRequestDTO request)
        {
            DTOValidationHelper.ValidateModelState(ModelState);

            // Retrieve employee by email
            var employee = (await _employeeService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "Email", Value = request.Email }
            })).FirstOrDefault();

            if (employee == null)
                return NotFound(new { Message = "Employee not found." });

            // Get start and end dates based on period
            DateTime startDate = request.Period == "weekly" ? DateTime.UtcNow.AddDays(-7) : DateTime.UtcNow.AddMonths(-1);
            DateTime endDate = DateTime.UtcNow;

            // Adjust filter to use the Date property for filtering TimeSheets
            var timeSheets = await _timeSheetService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "EmployeeId", Value = employee.EmployeeId },
                new FilterDTO { PropertyName = "Date", Value = startDate, Operator = ">=" },  // Use Date for filtering
                new FilterDTO { PropertyName = "Date", Value = endDate, Operator = "<=" }       // Use Date for filtering
            });

            // Get the number of leaves taken within the date range
            var leavesTaken = (await _leaveService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "EmployeeId", Value = employee.EmployeeId },
                new FilterDTO { PropertyName = "StartDate", Value = startDate, Operator = ">=" },
                new FilterDTO { PropertyName = "EndDate", Value = endDate, Operator = "<=" }
            })).Count();

            var department = await _departmentService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "DepartmentId", Value = employee.DepartmentId }
            });

            // Handle the case where department is not found
            var departmentName = department.FirstOrDefault()?.DepartmentName ?? "No Department";

            // Create the report DTO
            var report = new EmployeeWorkReportDTO
            {
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                Email = employee.Email,
                Department = departmentName,
                TotalHoursWorked = (int)timeSheets.Sum(ts => ts.TotalHours ?? 0),
                LeavesTaken = leavesTaken,
                PaidLeavesRemaining = employee.PaidLeavesRemaining,
                TimeSheets = timeSheets.Select(ts => new TimeSheetDTOForReport
                {
                    Date = ts.Date,
                    StartTime = ts.StartTime,
                    EndTime = ts.EndTime,
                    TotalHours = ts.TotalHours,
                    Description = ts.Description
                }).ToList()
            };

            // Generate PDF using PdfService
            byte[] pdfBytes = _pdfService.GenerateEmployeeWorkHoursReport(report, request.Period);

            // Return the PDF as a file
            return File(pdfBytes, "application/pdf", $"Employee_Work_Report_{employee.EmployeeId}.pdf");
        }


    }
}
