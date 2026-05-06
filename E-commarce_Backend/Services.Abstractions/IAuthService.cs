using E_commarce_Backend.Dtos.Auth;

namespace E_commarce_Backend.Services.Abstractions
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto model);
        Task<string> VerifyEmailAsync(VerifyEmailDto model);
        Task<string> ResendVerificationCodeAsync(ResendVerificationCodeDto model);

        Task<object> LoginAsync(LoginDto model);

        Task<string> ForgotPasswordAsync(ForgetPasswordDto model);
        Task<string> ResetPasswordAsync(ResetPasswordDto model);
        Task<string> ResendResetCodeAsync(ResendResetCodeDto model);
    }
}
