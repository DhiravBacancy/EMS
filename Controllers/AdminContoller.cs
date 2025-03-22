using EMS.Service;
using Microsoft.AspNetCore.Mvc;
using EMS.DTOs;
using EMS.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

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
            if (addAdminDto == null)
                return BadRequest(new { Message = "Invalid data" });

            var existingAdmin = (await _adminService.GetByMultipleConditionsAsync(
                new List<FilterDTO> { new FilterDTO { PropertyName = "Email", Value = addAdminDto.Email } }
            )).FirstOrDefault();

            if (existingAdmin != null)
                return BadRequest(new { Message = "Email is already in use" });

            var newAdmin = new Admin
            {
                FirstName = addAdminDto.FirstName,
                LastName = addAdminDto.LastName,
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

            await _adminService.DeleteAsync(admin.AdminId);
            return Ok(new { Message = "Admin deleted successfully" });
        }
    }

}
