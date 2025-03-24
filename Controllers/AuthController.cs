using Microsoft.AspNetCore.Mvc;
using EMS.Helpers;
using EMS.Service;
using EMS.DTOs;

[ApiController]
[Route("api/[controller]/[action]")]
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
        var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
        if (validationResult != null) throw new CustomException("Invalid request data", 400);

        var tokenResponse = await _authService.LoginAsync(loginDto);
        if (tokenResponse != null)
            return Ok(tokenResponse);
        else
            throw new CustomException("Invalid credentials", 401);
    }

    [HttpPost]
    public IActionResult Logout([FromBody] string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new CustomException("Token is required", 400);

        if (_authService.IsTokenRevoked(token))
            throw new CustomException("Token already revoked", 400);

        _authService.Logout(token);
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDto)
    {
        if (resetPasswordDto == null)
            throw new CustomException("Invalid request data", 400);

        var result = await _authService.PasswordReset(resetPasswordDto);
        if (result == null) throw new CustomException("Password reset failed", 500);

        return Ok(new { message = "Password reset successful" });
    }

    [HttpPost]
    public async Task<IActionResult> ResetPasswordWhenNotLoggedIn([FromBody] string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new CustomException("Email is required", 400);

        var result = await _authService.ResetPasswordWhenNotLoggedIn(email);
        if (result == null) throw new CustomException("Email not found", 404);

        return Ok(new { message = "Password reset instructions sent to email" });
    }

    [HttpPost]
    public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPDTO verifyOtpDto)
    {
        if (string.IsNullOrEmpty(verifyOtpDto.Email) || string.IsNullOrEmpty(verifyOtpDto.EnteredOTP))
            throw new CustomException("Email and OTP are required", 400);

        var response = await _authService.VerifyOTP(verifyOtpDto.Email, verifyOtpDto.EnteredOTP);
        if (!response) throw new CustomException("Invalid OTP", 400);

        return Ok(new { message = "OTP verified successfully" });
    }
}
