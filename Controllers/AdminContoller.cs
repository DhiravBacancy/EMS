using EMS.Service;
using Microsoft.AspNetCore.Mvc;
using EMS.DTOs;
using EMS.Models;
using EMS.Helpers;

namespace EMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IGenericDBService<Admin> _adminService;

        public AdminController(IGenericDBService<Admin> adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddAdmin([FromBody] AddAdminDTO addAdminDto)
        {
            // Call the validation helper method to check for ModelState errors
            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;

            var existingAdmin = (await _adminService.GetByMultipleConditionsAsync(
                new List<FilterDTO> { new FilterDTO { PropertyName = "Email", Value = addAdminDto.Email } }
            )).FirstOrDefault();

            if (existingAdmin != null)
                return BadRequest(new { Message = "Email is already in use" });

            var newAdmin = new Admin
            {
                FirstName = addAdminDto.FirstName,
                LastName = addAdminDto.LastName,
                Passsword = BCrypt.Net.BCrypt.HashPassword(addAdminDto.Passsword),
                Email = addAdminDto.Email,
                Phone = addAdminDto.Phone,
            };

            if (await _adminService.AddAsync(newAdmin))
                return Ok(new { Message = "Admin added successfully" });
            else
                return StatusCode(500, new { Message = "Failed to add Admin due to an internal server error." });
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllAdmins()
        {
            var admins = await _adminService.GetAllAsync();
            if (admins == null || !admins.Any())
                return NotFound(new { Message = "No Admin Found" });

            return Ok(admins);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdminById(int id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            if (admin == null)
                return NotFound(new { Message = "Admin not found" });

            return Ok(admin);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateAdmin(int id, [FromBody] UpdateAdminDTO updateAdminDto)
        {
            // Call the validation helper method to check for ModelState errors
            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;

            var existingAdmin = await _adminService.GetByIdAsync(id);
            if (existingAdmin == null)
                return NotFound(new { Message = "Admin not found" });

            var emailCheck = (await _adminService.GetByMultipleConditionsAsync(
                new List<FilterDTO> { new FilterDTO { PropertyName = "Email", Value = updateAdminDto.Email } }
            )).FirstOrDefault();

            if (emailCheck != null && emailCheck.AdminId != id)
                return BadRequest(new { Message = "Email is already in use by another admin" });

            existingAdmin.FirstName = updateAdminDto.FirstName ?? existingAdmin.FirstName;
            existingAdmin.LastName = updateAdminDto.LastName ?? existingAdmin.LastName;
            existingAdmin.Email = updateAdminDto.Email ?? existingAdmin.Email;
            existingAdmin.Phone = updateAdminDto.Phone ?? existingAdmin.Phone;
            existingAdmin.UpdatedAt = DateTime.UtcNow;

            if (await _adminService.UpdateAsync(existingAdmin))
                return Ok(new { Message = "Admin updated successfully" });
            else
                return StatusCode(500, new { Message = "Failed to update Admin due to an internal server error." });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            if (admin == null)
                return NotFound(new { Message = "Admin not found" });

            if (await _adminService.DeleteAsync(admin.AdminId))
                return Ok(new { Message = "Admin deleted successfully" });
            else
                return StatusCode(500, new { Message = "Failed to update Admin due to an internal server error. "});
        }
    }
}
