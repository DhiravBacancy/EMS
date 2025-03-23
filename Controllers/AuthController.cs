using EMS.DTOs;
using EMS.Helpers;
using EMS.Service;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            // Call the validation helper method to check for ModelState errors
            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;
            try
            {
                var tokenResponse = await _authService.LoginAsync(loginDto);
                if (tokenResponse == null)
                    return Unauthorized(new { message = "Invalid email or password"});
                else 
                    return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Invalid email or password", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Logout([FromBody] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token is required" });
            }
            _authService.Logout(token);
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDto)
        {
            if (resetPasswordDto == null)
            {
                return BadRequest(new { message = "Invalid request" });
            }
            try
            {
                var result = await _authService.PasswordReset(resetPasswordDto);
                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Password reset failed", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordWhenNotLoggedIn([FromBody] string email)
        {
            // Call the validation helper method to check for ModelState errors
            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;
            try
            {
                var result = await _authService.ResetPasswordWhenNotLoggedIn(email);
                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error sending OTP", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTP([FromBody] OTPVerificationDTO otpDto)
        {
            // Call the validation helper method to check for ModelState errors
            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;
            try
            {
                var result = await _authService.VerifyOTP(otpDto.Email, otpDto.EnteredOTP);
                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "OTP verification failed", error = ex.Message });
            }
        }
    }
}
