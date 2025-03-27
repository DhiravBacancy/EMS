using EMS.DTOs;
using EMS.Helpers;
using EMS.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EMS.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var loginResponse = await _authService.LoginAsync(loginDto);
            if (loginResponse.Success)
                return Ok(loginResponse.Data);  // Return token on successful login

            return Unauthorized(new { message = loginResponse.Message });  // Unauthorized error for invalid credentials
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout([FromBody] string token)
        {
            token = token?.Trim('"'); // Fix JSON wrapping issue

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            var logoutResponse = await _authService.LogoutAsync(token);
            if (logoutResponse.Success)
                return Ok(new { message = "Logged out successfully" });

            return StatusCode(500, new { message = "An error occurred while logging out" });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDto)
        {
            var resetPasswordResponse = await _authService.PasswordResetAsync(resetPasswordDto);
            if (resetPasswordResponse.Success)
                return Ok(new { message = "Password reset successful" });

            return StatusCode(500, new { message = resetPasswordResponse.Message });  // Return error if password reset fails
        }

        
        [HttpPost]
        public async Task<IActionResult> ResetPasswordWhenNotLoggedIn([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new { message = "Email is required" });

            var resetPasswordResponse = await _authService.ResetPasswordWhenNotLoggedInAsync(email);
            if (resetPasswordResponse.Success)
                return Ok(new { message = "Password reset instructions sent to your email" });

            return NotFound(new { message = "Email not found" });  // Return 404 if email is not found
        }

 
        [HttpPost]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPDTO verifyOtpDto)
        {
            if (string.IsNullOrEmpty(verifyOtpDto.Email) || string.IsNullOrEmpty(verifyOtpDto.EnteredOTP))
                return BadRequest(new { message = "Email and OTP are required" });

            var otpResponse = await _authService.VerifyOTPAsync(verifyOtpDto.Email, verifyOtpDto.EnteredOTP);
            if (otpResponse.Success)
            {
                var resetPasswordDto = new ResetPasswordDTO
                {
                    Email = verifyOtpDto.Email,
                    NewPassword = verifyOtpDto.NewPassword
                };

                var resetPasswordResponse = await _authService.PasswordResetAsync(resetPasswordDto);
                if (resetPasswordResponse.Success)
                    return Ok(new { message = "Password reset successful" });

                return StatusCode(500, new { message = resetPasswordResponse.Message });
            }

            return BadRequest(new { message = otpResponse.Message });  // Invalid OTP error
        }
    }
}
