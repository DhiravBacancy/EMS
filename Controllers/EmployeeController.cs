using EMS.DTOs;
using EMS.Helpers;
using EMS.Service.EMS.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] AddEmployeeDTO employeeDto)
        {
            

            var response = await _employeeService.AddEmployeeAsync(employeeDto);
            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(new { message = response.Message });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var response = await _employeeService.GetAllEmployeesAsync();
            if (!response.Success)
                return NotFound(new { message = response.Message });

            return Ok(response.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var response = await _employeeService.GetEmployeeByIdAsync(id);
            if (!response.Success)
                return NotFound(new { message = response.Message });

            return Ok(response.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UdpateEmployeeDTO updateEmployeeDto)
        {
            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;

            var response = await _employeeService.UpdateEmployeeAsync(id, updateEmployeeDto);
            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(new { message = response.Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var response = await _employeeService.DeleteEmployeeAsync(id);
            if (!response.Success)
                return NotFound(new { message = response.Message });

            return Ok(new { message = response.Message });
        }
    }
}
