namespace E_commarce_Backend.Services
{
    using System.Security.Cryptography;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using E_commarce_Backend.Models.User;
    using E_commarce_Backend.Data;
    using E_commarce_Backend.Services.Abstractions;
    using E_commarce_Backend.Dtos.Auth;

    public class AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        AppDbContext context,
        IEmailService emailService,
        IJwtService jwtService) : IAuthService
    {
        public async Task<string> RegisterAsync(RegisterDto model)
        {
            var existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                throw new Exception("Email already registered");

            var pending = await context.PendingUsers
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (pending != null)
                context.PendingUsers.Remove(pending);

            var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            var pendingUser = new PendingUser
            {
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Password = model.Password,
                VerificationCode = code,
                CodeExpiry = DateTime.UtcNow.AddMinutes(10)
            };

            context.PendingUsers.Add(pendingUser);
            await context.SaveChangesAsync();

            await emailService.SendEmailAsync(
                model.Email,
                "Verify your email",
                $"Your code is: {code}"
            );

            return "Verification code sent";
        }

        public async Task<string> VerifyEmailAsync(VerifyEmailDto model)
        {
            var pendingUser = await context.PendingUsers
                .FirstOrDefaultAsync(x => x.VerificationCode == model.Code);

            if (pendingUser == null)
                throw new Exception("Invalid code");

            if (pendingUser.CodeExpiry < DateTime.UtcNow)
                throw new Exception("Code expired");

            var user = new AppUser
            {
                UserName = pendingUser.Email,
                Email = pendingUser.Email,
                FullName = pendingUser.FullName,
                PhoneNumber = pendingUser.PhoneNumber,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, pendingUser.Password);
            if (!result.Succeeded)
                throw new Exception("User creation failed");

            await userManager.AddToRoleAsync(user, "Customer");

            context.PendingUsers.Remove(pendingUser);
            await context.SaveChangesAsync();

            return "Account created successfully";
        }

        public async Task<object> LoginAsync(LoginDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new Exception("Invalid credentials");

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                throw new Exception("Invalid credentials");

            if (!user.EmailConfirmed)
                throw new Exception("Email not verified");

            var roles = await userManager.GetRolesAsync(user);

            return new
            {
                Token = await jwtService.GenerateToken(user),
                User = new
                {
                    user.Id,
                    user.Email,
                    user.FullName,
                    Roles = roles
                }
            };
        }

        public async Task<string> ForgotPasswordAsync(ForgetPasswordDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return "If exists, code sent";

            var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            user.PasswordResetCode = code;
            user.PasswordResetCodeExpiry = DateTime.UtcNow.AddMinutes(10);

            await userManager.UpdateAsync(user);

            await emailService.SendEmailAsync(model.Email, "Reset Code", $"Code: {code}");

            return "Reset code sent";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto model)
        {
            if (model.NewPassword != model.ConfirmNewPassword)
                throw new Exception("Passwords do not match");

            var user = await userManager.Users
                .FirstOrDefaultAsync(x => x.PasswordResetCode == model.Code);

            if (user == null)
                throw new Exception("Invalid code");

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            await userManager.ResetPasswordAsync(user, token, model.NewPassword);

            user.PasswordResetCode = null;
            user.PasswordResetCodeExpiry = null;

            await userManager.UpdateAsync(user);

            return "Password reset successful";
        }

        public async Task<string> ResendVerificationCodeAsync(ResendVerificationCodeDto model)
        {
            var pendingUser = await context.PendingUsers
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (pendingUser == null)
                throw new Exception("No pending registration");

            var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            pendingUser.VerificationCode = code;
            pendingUser.CodeExpiry = DateTime.UtcNow.AddMinutes(10);

            await context.SaveChangesAsync();

            await emailService.SendEmailAsync(model.Email, "Verification Code", $"Code: {code}");

            return "Code resent";
        }

        public async Task<string> ResendResetCodeAsync(ResendResetCodeDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new Exception("User not found");

            var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            user.PasswordResetCode = code;
            user.PasswordResetCodeExpiry = DateTime.UtcNow.AddMinutes(10);

            await userManager.UpdateAsync(user);

            await emailService.SendEmailAsync(model.Email, "Reset Code", $"Code: {code}");

            return "Reset code resent";
        }
    }
}
