using EMS.Data;
using EMS.DTOs;
using EMS.Enums;
using EMS.HelperClasses;
using EMS.Helpers;
using EMS.Models;
using EMS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.Service
{
    public interface IAdminService
    {
        Task<ServiceResponse<Admin>> AddAdminAsync(AddAdminDTO addAdminDto);
        Task<ServiceResponse<Admin>> GetAdminByIdAsync(int id);
        Task<ServiceResponse<Admin>> UpdateAdminAsync(int id, UpdateAdminDTO updateAdminDto);
        Task<ServiceResponse<IEnumerable<Employee>>> GetEmployeeDetailByEmailAsync(string employeeEmail);
        Task<ServiceResponse<IEnumerable<LeaveTransferDTO>>> GetEmployeePendingLeaveRequestsAsync(string employeeEmail);
        Task<ServiceResponse<FileContentResult>> ExportEmployeeTimeSheetAsync(string employeeEmail);
        Task<ServiceResponse<FileContentResult>> GenerateEmployeeWorkReportAsync(EmployeeWorkReportRequestDTO request);
    }

    public class AdminService : IAdminService
    {
        private readonly IGenericDBService<Admin> _adminService;
        private readonly IGenericDBService<TimeSheet> _timeSheetService;
        private readonly IGenericDBService<Employee> _employeeService;
        private readonly IGenericDBService<Leave> _leaveService;
        private readonly IGenericDBService<Department> _departmentService;
        private readonly IExportTimesheetsToExcelService _exportToExcelService;
        private readonly IPdfService _pdfService;

        public AdminService(
            IGenericDBService<TimeSheet> timeSheetService,
            IGenericDBService<Employee> employeeService,
            IGenericDBService<Leave> leaveService,
            IGenericDBService<Department> departmentService,
            IExportTimesheetsToExcelService exportToExcelService,
            IPdfService pdfService,
            IGenericDBService<Admin> adminService)
        {
            _timeSheetService = timeSheetService;
            _employeeService = employeeService;
            _leaveService = leaveService;
            _departmentService = departmentService;
            _exportToExcelService = exportToExcelService;
            _pdfService = pdfService;
            _adminService = adminService;
        }

        public async Task<ServiceResponse<Admin>> AddAdminAsync(AddAdminDTO addAdminDto)
        {
            var existingAdmin = (await _adminService.GetByMultipleConditionsAsync(
                new List<FilterDTO> { new FilterDTO { PropertyName = "Email", Value = addAdminDto.Email } }
            )).FirstOrDefault();

            if (existingAdmin != null)
                return ServiceResponse<Admin>.FailureResponse("Email is already in use", 400);

            var newAdmin = new Admin
            {
                FirstName = addAdminDto.FirstName,
                LastName = addAdminDto.LastName,
                Passsword = BCrypt.Net.BCrypt.HashPassword(addAdminDto.Passsword),
                Email = addAdminDto.Email,
                Phone = addAdminDto.Phone,
            };

            await _adminService.AddAsync(newAdmin);
            return ServiceResponse<Admin>.SuccessResponse(newAdmin, "Admin added successfully", 201);
        }

        public async Task<ServiceResponse<Admin>> GetAdminByIdAsync(int id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            if (admin == null)
                return ServiceResponse<Admin>.FailureResponse("Admin not found", 404);

            return ServiceResponse<Admin>.SuccessResponse(admin, "Admin fetched successfully", 200);
        }

        public async Task<ServiceResponse<Admin>> UpdateAdminAsync(int id, UpdateAdminDTO updateAdminDto)
        {
            var existingAdmin = await _adminService.GetByIdAsync(id);
            if (existingAdmin == null)
                return ServiceResponse<Admin>.FailureResponse("Admin not found", 404);

            var emailCheck = (await _adminService.GetByMultipleConditionsAsync(
                new List<FilterDTO> { new FilterDTO { PropertyName = "Email", Value = updateAdminDto.Email } }
            )).FirstOrDefault();

            if (emailCheck != null && emailCheck.AdminId != id)
                return ServiceResponse<Admin>.FailureResponse("Email is already in use by another admin", 400);

            existingAdmin.FirstName = updateAdminDto.FirstName ?? existingAdmin.FirstName;
            existingAdmin.LastName = updateAdminDto.LastName ?? existingAdmin.LastName;
            existingAdmin.Email = updateAdminDto.Email ?? existingAdmin.Email;
            existingAdmin.Phone = updateAdminDto.Phone ?? existingAdmin.Phone;
            existingAdmin.UpdatedAt = DateTime.UtcNow;

            await _adminService.UpdateAsync(existingAdmin);
            return ServiceResponse<Admin>.SuccessResponse(existingAdmin, "Admin updated successfully", 200);
        }

        public async Task<ServiceResponse<IEnumerable<Employee>>> GetEmployeeDetailByEmailAsync(string employeeEmail)
        {
            var employees = await _employeeService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "Email", Value = employeeEmail }
            });

            if (employees == null || !employees.Any())
                return ServiceResponse<IEnumerable<Employee>>.FailureResponse("Employee not found", 404);

            return ServiceResponse<IEnumerable<Employee>>.SuccessResponse(employees, "Employee details fetched successfully", 200);
        }

        public async Task<ServiceResponse<IEnumerable<LeaveTransferDTO>>> GetEmployeePendingLeaveRequestsAsync(string employeeEmail)
        {
            var employees = await _employeeService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "Email", Value = employeeEmail }
            });

            if (employees == null || !employees.Any())
                return ServiceResponse<IEnumerable<LeaveTransferDTO>>.FailureResponse("No Employee found for export.", 400);

            var employeeId = employees.First().EmployeeId;
            var pendingLeaves = await _leaveService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "EmployeeId", Value = employeeId },
                new FilterDTO { PropertyName = "Status", Value = StatusEnum.Pending }
            });

            if (pendingLeaves == null || !pendingLeaves.Any())
                return ServiceResponse<IEnumerable<LeaveTransferDTO>>.FailureResponse("No pending leave requests found.", 400);

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

            return ServiceResponse<IEnumerable<LeaveTransferDTO>>.SuccessResponse(leaveDTOs, "Pending leave requests fetched successfully", 200);
        }

        public async Task<ServiceResponse<FileContentResult>> ExportEmployeeTimeSheetAsync(string employeeEmail)
        {
            var employees = await _employeeService.GetByMultipleConditionsAsync(new List<FilterDTO>
        {
            new FilterDTO { PropertyName = "Email", Value = employeeEmail }
        });

            if (employees == null || !employees.Any())
                return ServiceResponse<FileContentResult>.FailureResponse("No Employee found for export.", 400);

            var employeeId = employees.First().EmployeeId;
            var timeSheets = await _timeSheetService.GetByMultipleConditionsAsync(new List<FilterDTO>
        {
            new FilterDTO { PropertyName = "EmployeeId", Value = employeeId }
        });

            if (timeSheets == null || !timeSheets.Any())
                return ServiceResponse<FileContentResult>.FailureResponse("No timesheets found for export.", 400);

            var timeSheetList = timeSheets.ToList();
            var fileResult = await _exportToExcelService.ExportToExcel(timeSheetList);
            return ServiceResponse<FileContentResult>.SuccessResponse(fileResult, "Timesheets exported successfully", 200);
        }

        public async Task<ServiceResponse<FileContentResult>> GenerateEmployeeWorkReportAsync(EmployeeWorkReportRequestDTO request)
        {
            var employee = (await _employeeService.GetByMultipleConditionsAsync(new List<FilterDTO>
        {
            new FilterDTO { PropertyName = "Email", Value = request.Email }
        })).FirstOrDefault();

            if (employee == null)
                return ServiceResponse<FileContentResult>.FailureResponse("Employee not found", 404);

            DateTime startDate = request.Period == "weekly" ? DateTime.UtcNow.AddDays(-7) : DateTime.UtcNow.AddMonths(-1);
            DateTime endDate = DateTime.UtcNow;

            var timeSheets = await _timeSheetService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "EmployeeId", Value = employee.EmployeeId },
                new FilterDTO { PropertyName = "Date", Value = startDate, Operator = ">=" },
                new FilterDTO { PropertyName = "Date", Value = endDate, Operator = "<=" }
            });

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
            var departmentName = department.FirstOrDefault()?.DepartmentName ?? "No Department";

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

            byte[] pdfBytes = _pdfService.GenerateEmployeeWorkHoursReport(report, request.Period);
            return ServiceResponse<FileContentResult>.SuccessResponse(new FileContentResult(pdfBytes, "application/pdf")
            {
                FileDownloadName = $"Employee_Work_Report_{employee.EmployeeId}.pdf"
            }, "Employee work report generated successfully", 200);
        }
    }

}
