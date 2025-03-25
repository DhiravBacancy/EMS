using EMS.DTOs;
using EMS.Helpers;
using EMS.Models;
using EMS.Service;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace EMS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IGenericDBService<Employee> _employeeService;
        private readonly IEmployeeService _employeeService1;
        private readonly IPdfService _pdfService;

        public EmployeeController(IGenericDBService<Employee> employeeService, IPdfService pdfService, IEmployeeService employeeService1)
        {
            _employeeService = employeeService;
            _pdfService = pdfService;
            _employeeService1 = employeeService1;
        }

        // Create Employee
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] AddEmployeeDTO employeeDto)
        {
            // Call the validation helper method to check for ModelState errors
            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;

            var existingEmployee = (await _employeeService.GetByMultipleConditionsAsync(
                new List<FilterDTO>
                {
                    new FilterDTO { PropertyName = "Email", Value = employeeDto.Email }
                }
            )).FirstOrDefault();

            if (existingEmployee != null)
                return BadRequest(new { message = "Email is already in use" });

            // Create new employee
            var newEmployee = new Employee
            {
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(employeeDto.Password), // Hash password
                Phone = employeeDto.Phone,
                DateOfBirth = employeeDto.DateOfBirth,
                Address = employeeDto.Address,
                TechStack = employeeDto.TechStack
            };

            if (await _employeeService.AddAsync(newEmployee))
                return Ok(new { message = "Employee added successfully" });

            return StatusCode(500, new { message = "Failed to add Employee due to an internal server error." });
        }

        // Get All Employees
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllAsync();

            if (employees == null || !employees.Any())
                return NotFound(new { message = "No employees found" });

            return Ok(employees);
        }

        // Get Employee by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);

            if (employee == null)
                return NotFound(new { message = "Employee not found" });

            return Ok(employee);
        }

        // Update Employee
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UdpateEmployeeDTO updateEmployeeDto)
        {

            // Call the validation helper method to check for ModelState errors
            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;


            var existingEmployee = await _employeeService.GetByIdAsync(id);
            if (existingEmployee == null)
                return NotFound(new { message = "Employee not found" });

            // Check if email is taken by another employee using the new filter method
            if (!string.IsNullOrEmpty(updateEmployeeDto.Email))
            {
                var emailCheck = (await _employeeService.GetByMultipleConditionsAsync(
                    new List<FilterDTO>
                    {
                        new FilterDTO { PropertyName = "Email", Value = updateEmployeeDto.Email }
                    }
                )).FirstOrDefault();

                if (emailCheck != null && emailCheck.EmployeeId != id)
                    return BadRequest(new { message = "Email is already in use by another employee" });
            }

            // Update fields
            existingEmployee.Password = BCrypt.Net.BCrypt.HashPassword(updateEmployeeDto.Password);
            existingEmployee.FirstName = updateEmployeeDto.FirstName ?? existingEmployee.FirstName;
            existingEmployee.LastName = updateEmployeeDto.LastName ?? existingEmployee.LastName;
            existingEmployee.Email = updateEmployeeDto.Email ?? existingEmployee.Email;
            existingEmployee.Phone = updateEmployeeDto.Phone ?? existingEmployee.Phone;
            existingEmployee.DateOfBirth = updateEmployeeDto.DateOfBirth ?? existingEmployee.DateOfBirth;
            existingEmployee.Address = updateEmployeeDto.Address ?? existingEmployee.Address;
            existingEmployee.TechStack = updateEmployeeDto.TechStack ?? existingEmployee.TechStack;
            existingEmployee.UpdatedAt = DateTime.UtcNow;

            if (await _employeeService.UpdateAsync(existingEmployee))
                return Ok(new { message = "Employee updated successfully" });

            return StatusCode(500, new { message = "Failed to update Employee due to an internal server error." });
        }

        // Delete Employee
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
                return NotFound(new { message = "Employee not found" });
            if (await _employeeService.DeleteAsync(employee.EmployeeId))
                return Ok(new { message = "Employee deleted successfully" });
            else
                return StatusCode(500, new { message = "Failed to update Employee due to an internal server error." });
        }

        [HttpGet("workhours")]
        // Ensure the method is async
        public async Task<IActionResult> GenerateEmployeeReport(int employeeId, string reportType)
        {
            try
            {
                // Await the async call to get employee details
                var employeeDetails = await _employeeService1.GetEmployeeDetailsAsync(employeeId, reportType);

                // Generate PDF using the resolved employee details
                var pdfBytes = _pdfService.GenerateEmployeeWorkHoursReport(employeeDetails, reportType);

                // Return the PDF as a file (or any other response depending on your requirement)
                return File(pdfBytes, "application/pdf", "EmployeeWorkHoursReport.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("admin-dashboard/employee")]
        public async Task<IActionResult> GetEmployeeDetailsForAdmin([FromQuery] int employeeId)
        {
            var employeeData = await _employeeService1.GetAdminDashboardDataAsync(employeeId);

            if (employeeData == null)
            {
                return NotFound("Employee data not found.");
            }

            // Generate HTML response with CSS
            var htmlContent = $@"
<html>
<head>
    <title>Employee Details - Admin</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; padding: 20px; background-color: #f4f4f4; }}
        h2 {{ color: #2c3e50; text-align: center; }}
        h3 {{ color: #34495e; }}
        table {{ width: 100%; border-collapse: collapse; margin-top: 20px; background: white; }}
        th, td {{ border: 1px solid #ddd; padding: 10px; text-align: left; }}
        th {{ background-color: #2c3e50; color: white; }}
        .container {{ max-width: 800px; margin: auto; background: white; padding: 20px; border-radius: 8px; box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1); }}
        .section {{ margin-bottom: 30px; padding: 15px; border: 1px solid #ddd; border-radius: 5px; background: #fff; }}
    </style>
</head>
<body>
    <div class='container'>
        <h2>Employee Details</h2>

        <div class='section'>
            <h3>Name: {employeeData.TopEmployees[0].EmployeeName}</h3>
            <p><strong>Email:</strong> {employeeData.TopEmployees[0].EmployeeEmail}</p>
            
        </div>

        <div class='section'>
            <h3>Total Logged Hours: {employeeData.TotalLoggedHours}</h3>
        </div>

        <div class='section'>
            <h3>Pending Leave Requests</h3>
            <table>
                <tr>
                    <th>Start Date</th>
                    <th>End Date</th>
                    <th>Reason</th>
                    <th>Status</th>
                </tr>";

            foreach (var leave in employeeData.PendingLeaveRequests)
            {
                htmlContent += $@"
                <tr>
                    <td>{leave.StartDate}</td>
                    <td>{leave.EndDate}</td>
                    <td>{leave.Reason}</td>
                    <td>{leave.Status}</td>
                </tr>";
            }

            htmlContent += @"
            </table>
        </div>

        <div class='section'>
            <h3>Recent Timesheets</h3>
            <table>
                <tr>
                    <th>Date</th>
                    <th>Hours Worked</th>
                </tr>";

            //foreach (var timesheet in employeeData.TopEmployees[0])
            //{
            //    htmlContent += $@"
            //    <tr>
            //        <td>{timesheet.Date}</td>
            //        <td>{timesheet.HoursWorked}</td>
            //    </tr>";
            //}

            htmlContent += @"
            </table>
        </div>
    </div>
</body>
</html>";

            return Content(htmlContent, "text/html");
        }


    }
}
