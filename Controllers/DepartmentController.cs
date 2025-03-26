using EMS.DTOs;
using EMS.Helpers;
using EMS.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Controllers
{

    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // Add Department
        [HttpPost("Add")]
        public async Task<IActionResult> AddDepartment([FromBody] AddOrUpdateDepartmentDTO departmentDto)
        {
            var response = await _departmentService.AddDepartmentAsync(departmentDto);
            if (response.Success)
                return StatusCode(response.StatusCode, new { message = response.Message, data = response.Data });

            return StatusCode(response.StatusCode, new { message = response.Message });
        }

        // Get All Departments
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllDepartments()
        {
            var response = await _departmentService.GetAllDepartmentsAsync();
            if (response.Success)
                return Ok(new { message = response.Message, data = response.Data });

            return StatusCode(response.StatusCode, new { message = response.Message });
        }

        // Get Department by ID
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var response = await _departmentService.GetDepartmentByIdAsync(id);
            if (response.Success)
                return Ok(new { message = response.Message, data = response.Data });

            return StatusCode(response.StatusCode, new { message = response.Message });
        }

        // Update Department
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] AddOrUpdateDepartmentDTO departmentDto)
        {
            var response = await _departmentService.UpdateDepartmentAsync(id, departmentDto);
            if (response.Success)
                return StatusCode(response.StatusCode, new { message = response.Message, data = response.Data });

            return StatusCode(response.StatusCode, new { message = response.Message });
        }
    }
}
