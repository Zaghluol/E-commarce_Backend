using E_commarce_Backend.Dtos.Auth;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
        => Ok(await authService.RegisterAsync(dto));

    [HttpPost("verify-email")]
    public async Task<IActionResult> Verify(VerifyEmailDto dto)
        => Ok(await authService.VerifyEmailAsync(dto));
   
    [HttpPost("resend-verification-code")]
    public async Task<IActionResult> ResendVerificationCode(ResendVerificationCodeDto dto)
    => Ok(await authService.ResendVerificationCodeAsync(dto));

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
        => Ok(await authService.LoginAsync(dto));

    [HttpPost("forgot-password")]
    public async Task<IActionResult> Forgot(ForgetPasswordDto dto)
        => Ok(await authService.ForgotPasswordAsync(dto));

    [HttpPost("reset-password")]
    public async Task<IActionResult> Reset(ResetPasswordDto dto)
        => Ok(await authService.ResetPasswordAsync(dto));

    [HttpPost("resend-reset-code")]
    public async Task<IActionResult> ResendResetCode(ResendResetCodeDto dto)
        => Ok(await authService.ResendResetCodeAsync(dto));
}