using EMS.DTOs;
using EMS.Helpers;
using EMS.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost("Add")]
    public async Task<IActionResult> AddAdmin([FromBody] AddAdminDTO addAdminDto)
    {
        var response = await _adminService.AddAdminAsync(addAdminDto);

        if (response.Success)
        {
            return StatusCode(response.StatusCode, new
            {
                message = response.Message,
                data = response.Data
            });
        }

        return StatusCode(response.StatusCode, new { message = response.Message });
    }


    [HttpGet("GetAdminById")]
    public async Task<IActionResult> GetAdminById([FromQuery] int id)
    {
        var response = await _adminService.GetAdminByIdAsync(id);

        if (response.Success)
        {
            return Ok(new { message = response.Message, data = response.Data });
        }

        return StatusCode(response.StatusCode, new { message = response.Message });
    }


    [HttpPut("Update/{id}")]
    public async Task<IActionResult> UpdateAdmin(int id, [FromBody] UpdateAdminDTO updateAdminDto)
    {
        var response = await _adminService.UpdateAdminAsync(id, updateAdminDto);

        if (response.Success)
        {
            return StatusCode(response.StatusCode, new { message = response.Message, data = response.Data });
        }

        return StatusCode(response.StatusCode, new { message = response.Message });
    }


    [HttpGet("getEmployeeDetail/{employeeEmail}")]
    public async Task<IActionResult> GetEmployeeDetail(string employeeEmail)
    {
        var response = await _adminService.GetEmployeeDetailByEmailAsync(employeeEmail);

        if (response.Success)
        {
            return Ok(new { message = response.Message, data = response.Data });
        }

        return StatusCode(response.StatusCode, new { message = response.Message });
    }


    [HttpGet("getEmployeePendingLeaveRequest/{employeeEmail}")]
    public async Task<IActionResult> GetEmployeesPendingLeaveRequest(string employeeEmail)
    {
        var response = await _adminService.GetEmployeePendingLeaveRequestsAsync(employeeEmail);

        if (response.Success)
        {
            return Ok(new { message = response.Message, data = response.Data });
        }

        return StatusCode(response.StatusCode, new { message = response.Message });
    }

    [HttpGet("exportEmployeeTimeSheet/{employeeEmail}")]
    public async Task<IActionResult> ExportTimeSheets(string employeeEmail)
    {
        var response = await _adminService.ExportEmployeeTimeSheetAsync(employeeEmail);

        if (response.Success)
        {
            return File(response.Data.FileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Employee_TimeSheet_{employeeEmail}.xlsx");
        }

        return StatusCode(response.StatusCode, new { message = response.Message });
    }


    [HttpPost("generateEmployeeWorkReport")]
    public async Task<IActionResult> GenerateEmployeeWorkReport([FromBody] EmployeeWorkReportRequestDTO request)
    {
        var response = await _adminService.GenerateEmployeeWorkReportAsync(request);

        if (response.Success)
        {
            return File(response.Data.FileContents, "application/pdf", $"Employee_Work_Report_{request.Email}.pdf");
        }

        return StatusCode(response.StatusCode, new { message = response.Message });
    }
}
