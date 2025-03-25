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
        //private readonly IEmployeeService _employeeService1;
        private readonly IPdfService _pdfService;

        public EmployeeController(IGenericDBService<Employee> employeeService, IPdfService pdfService)
        {
            _employeeService = employeeService;
            _pdfService = pdfService;
            //_employeeService1 = employeeService1;
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

    }
}
