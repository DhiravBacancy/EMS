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
        // Call the service method and get the response
        var response = await _adminService.AddAdminAsync(addAdminDto);

        // Handle the response based on its success or failure
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


    [HttpGet("{id}")]
    public async Task<IActionResult> GetAdminById([FromQuery] int id)
    {
        // Call the service method and get the response
        var response = await _adminService.GetAdminByIdAsync(id);

        // Return appropriate response based on success or failure
        if (response.Success)
        {
            return Ok(new { message = response.Message, data = response.Data });
        }

        return StatusCode(response.StatusCode, new { message = response.Message });
    }


    [HttpPut("Update/{id}")]
    public async Task<IActionResult> UpdateAdmin(int id, [FromBody] UpdateAdminDTO updateAdminDto)
    {
        // Call the service method and get the response
        var response = await _adminService.UpdateAdminAsync(id, updateAdminDto);

        // Return appropriate response based on success or failure
        if (response.Success)
        {
            return StatusCode(response.StatusCode, new { message = response.Message, data = response.Data });
        }

        return StatusCode(response.StatusCode, new { message = response.Message });
    }


    [HttpGet("getEmployeeDetail/{employeeEmail}")]
    public async Task<IActionResult> GetEmployeeDetail(string employeeEmail)
    {
        // Call the service method and get the response
        var response = await _adminService.GetEmployeeDetailByEmailAsync(employeeEmail);

        // Return appropriate response based on success or failure
        if (response.Success)
        {
            return Ok(new { message = response.Message, data = response.Data });
        }

        return StatusCode(response.StatusCode, new { message = response.Message });
    }


    [HttpGet("getEmployeePendingLeaveRequest/{employeeEmail}")]
    public async Task<IActionResult> GetEmployeesPendingLeaveRequest(string employeeEmail)
    {
        // Call the service method and get the response
        var response = await _adminService.GetEmployeePendingLeaveRequestsAsync(employeeEmail);

        // Return appropriate response based on success or failure
        if (response.Success)
        {
            return Ok(new { message = response.Message, data = response.Data });
        }

        return StatusCode(response.StatusCode, new { message = response.Message });
    }

    [HttpGet("exportEmployeeTimeSheet/{employeeEmail}")]
    public async Task<IActionResult> ExportTimeSheets(string employeeEmail)
    {
        // Call the service method and get the response
        var response = await _adminService.ExportEmployeeTimeSheetAsync(employeeEmail);

        // Return the file content if the response is successful
        if (response.Success)
        {
            return File(response.Data.FileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Employee_TimeSheet_{employeeEmail}.xlsx");
        }

        return StatusCode(response.StatusCode, new { message = response.Message });
    }


    [HttpGet("generateEmployeeWorkReport")]
    public async Task<IActionResult> GenerateEmployeeWorkReport([FromBody] EmployeeWorkReportRequestDTO request)
    {
        var response = await _adminService.GenerateEmployeeWorkReportAsync(request);

        // Return appropriate response based on success or failure
        if (response.Success)
        {
            return File(response.Data.FileContents, "application/pdf", $"Employee_Work_Report_{request.Email}.pdf");
        }

        return StatusCode(response.StatusCode, new { message = response.Message });
    }
}
