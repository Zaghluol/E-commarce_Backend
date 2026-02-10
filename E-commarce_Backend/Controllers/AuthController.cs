using E_commarce_Backend.Dtos;
using E_commarce_Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using E_commarce_Backend.Services;
using Microsoft.EntityFrameworkCore;
using E_commarce_Backend.Services.Abstractions;
using System.Security.Cryptography;
using E_commarce_Backend.Data;

namespace E_commarce_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        AppDbContext context,
        IConfiguration configuration ,
        IJwtService jwtService) : ControllerBase
    {

        //    [HttpPost("register")]
        //    public async Task<IActionResult> Register(
        //[FromBody] RegisterDto model,
        //[FromServices] IEmailService emailService)
        //    {
        //        // 1️⃣ Validate DTO
        //        if (!ModelState.IsValid)
        //            return BadRequest(ModelState);

        //        // 2️⃣ Check email
        //        var existingUser = await userManager.FindByEmailAsync(model.Email);
        //        if (existingUser != null)
        //            return BadRequest(new { Message = "Email is already registered." });

        //        // 3️⃣ Create user
        //        var user = new AppUser
        //        {
        //            UserName = model.Email,
        //            Email = model.Email,
        //            FullName = model.FullName,
        //            PhoneNumber = model.PhoneNumber,
        //            EmailConfirmed = false
        //        };

        //        var result = await userManager.CreateAsync(user, model.Password);
        //        if (!result.Succeeded)
        //            return BadRequest(result.Errors);

        //        // 4️⃣ Assign role
        //        await userManager.AddToRoleAsync(user, "Customer");

        //        // 5️⃣ Generate secure 6-digit verification code
        //        var code = RandomNumberGenerator
        //            .GetInt32(100000, 999999)
        //            .ToString();

        //        user.EmailVerificationCode = code;
        //        user.EmailCodeExpiry = DateTime.UtcNow.AddMinutes(10);
        //        user.LastVerificationCodeSentAt = DateTime.UtcNow;


        //        await userManager.UpdateAsync(user);

        //        // 6️⃣ Send verification email
        //        await emailService.SendEmailAsync(
        //            user.Email,
        //            "Verify your email",
        //           $@"
        //            <h3>Email Verification</h3>
        //            <p>Your verification code is:</p>
        //            <h2>{code}</h2>
        //            <p>This code expires in 10 minutes.</p>
        //            "
        //        );

        //        return Ok(new
        //        {
        //            Message = "Registration successful. Please verify your email."
        //        });
        //    }

        //    [HttpPost("verify-email")]
        //    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto model)
        //    {
        //        var user = await userManager.Users
        //            .FirstOrDefaultAsync(u =>
        //                u.EmailVerificationCode == model.Code);

        //        if (user == null)
        //            return BadRequest("Invalid verification code");

        //        if (user.EmailConfirmed)
        //            return BadRequest("Email already verified");

        //        if (user.EmailCodeExpiry < DateTime.UtcNow)
        //            return BadRequest("Verification code expired");

        //        user.EmailConfirmed = true;
        //        user.EmailVerificationCode = null;
        //        user.EmailCodeExpiry = null;

        //        await userManager.UpdateAsync(user);

        //        return Ok(new { Message = "Email verified successfully" });
        //    }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
    [FromBody] RegisterDto model,
    [FromServices] IEmailService emailService)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if already registered
            var existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest("Email is already registered.");

            // Remove old pending registration
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
                PasswordHash = userManager.PasswordHasher
                    .HashPassword(null!, model.Password),
                VerificationCode = code,
                CodeExpiry = DateTime.UtcNow.AddMinutes(10)
            };

            context.PendingUsers.Add(pendingUser);
            await context.SaveChangesAsync();

            await emailService.SendEmailAsync(
                model.Email,
                "Verify your email",
                $@"
        <h3>Email Verification</h3>
        <p>Your verification code is:</p>
        <h2>{code}</h2>
        <p>This code expires in 10 minutes.</p>
        "
            );

            return Ok("Verification code sent to your email.");
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto model)
        {
            var pendingUser = await context.PendingUsers
                .FirstOrDefaultAsync(x => x.VerificationCode == model.Code);

            if (pendingUser == null)
                return BadRequest("Invalid verification code");

            if (pendingUser.CodeExpiry < DateTime.UtcNow)
                return BadRequest("Verification code expired");

            var user = new AppUser
            {
                UserName = pendingUser.Email,
                Email = pendingUser.Email,
                FullName = pendingUser.FullName,
                PhoneNumber = pendingUser.PhoneNumber,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Set password
            await userManager.AddPasswordAsync(user, pendingUser.PasswordHash);

            await userManager.AddToRoleAsync(user, "Customer");

            // Remove pending record
            context.PendingUsers.Remove(pendingUser);
            await context.SaveChangesAsync();

            return Ok("Email verified and account created successfully.");
        }


        [HttpPost("resend-verification-code")]
        public async Task<IActionResult> ResendVerificationCode(
        [FromBody] ResendVerificationCodeDto model,
        [FromServices] IEmailService emailService)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("User not found");

            if (user.EmailConfirmed)
                return BadRequest("Email already verified");

            // ⏱ Cooldown check (60 seconds)
            if (user.LastVerificationCodeSentAt.HasValue &&
                DateTime.UtcNow < user.LastVerificationCodeSentAt.Value.AddSeconds(60))
            {
                var waitTime =
                    (int)(user.LastVerificationCodeSentAt.Value
                    .AddSeconds(60) - DateTime.UtcNow).TotalSeconds;

                return BadRequest(new
                {
                    Message = $"Please wait {waitTime} seconds before requesting another code."
                });
            }

            // 🔐 Generate new secure code
            var code = RandomNumberGenerator
                .GetInt32(100000, 999999)
                .ToString();

            user.EmailVerificationCode = code;
            user.EmailCodeExpiry = DateTime.UtcNow.AddMinutes(10);
            user.LastVerificationCodeSentAt = DateTime.UtcNow;

            await userManager.UpdateAsync(user);

            // 📧 Send email
            await emailService.SendEmailAsync(
                user.Email,
                "Resend Email Verification Code",
                $@"
        <h3>Email Verification</h3>
        <p>Your new verification code is:</p>
        <h2>{code}</h2>
        <p>This code expires in 10 minutes.</p>
        "
            );

            return Ok(new
            {
                Message = "Verification code resent successfully"
            });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return Unauthorized(new { Message = "Invalid login credentials" });

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded)
                return Unauthorized(new { Message = "Invalid login credentials" });

            if (!user.EmailConfirmed)
                return Unauthorized("Please verify your email first");

            var roles = await userManager.GetRolesAsync(user);

            return Ok(new
            {
                Token = await jwtService.GenerateToken(user),
                User = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    Roles = roles
                }
            });
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
       [FromBody] ForgetPasswordDto model,
       [FromServices] IEmailService emailService)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Ok(new { Message = "If the account exists, a code has been sent to the email." });

            var random = new Random();
            var code = random.Next(100000, 999999).ToString();

            user.PasswordResetCode = code;
            user.PasswordResetCodeExpiry = DateTime.UtcNow.AddMinutes(10);
            user.LastResetCodeSentAt = DateTime.UtcNow;

            await userManager.UpdateAsync(user);

            var htmlContent = $@"
        <h2>Password Reset Code</h2>
        <p>Your 6-digit code is:</p>
        <h1 style='color:#2E86DE; font-size:32px;'>{code}</h1>
        <p>This code expires in 10 minutes.</p>";

            await emailService.SendEmailAsync(model.Email, "Password Reset Code", htmlContent);

            return Ok(new { Message = "If the email is registered, a reset code has been sent." });
        }



        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (model.NewPassword != model.ConfirmNewPassword)
                return BadRequest("Passwords do not match.");

            // Find the user who has this reset code
            var user = await userManager.Users
                .FirstOrDefaultAsync(u => u.PasswordResetCode == model.Code);

            if (user == null)
                return BadRequest("Invalid code.");

            if (user.PasswordResetCodeExpiry == null || DateTime.UtcNow > user.PasswordResetCodeExpiry)
                return BadRequest("Reset code expired.");

            // Proceed with password reset
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Clear code after success
            user.PasswordResetCode = null;
            user.PasswordResetCodeExpiry = null;
            await userManager.UpdateAsync(user);

            return Ok(new { Message = "Password reset successfully." });
        }

        [HttpPost("resend-Reset-code")]
        public async Task<IActionResult> ResendResetCode(
        [FromBody] ResendResetCodeDto model,
        [FromServices] IEmailService emailService)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("User not found");

            // ⏱ Cooldown check (60 seconds)
            if (user.LastResetCodeSentAt.HasValue &&
                DateTime.UtcNow < user.LastResetCodeSentAt.Value.AddSeconds(60))
            {
                var waitTime =
                    (int)(user.LastResetCodeSentAt.Value
                    .AddSeconds(60) - DateTime.UtcNow).TotalSeconds;

                return BadRequest(new
                {
                    Message = $"Please wait {waitTime} seconds before requesting another code."
                });
            }

            // 🔐 Generate new secure code
            var code = RandomNumberGenerator
                .GetInt32(100000, 999999)
                .ToString();

            user.PasswordResetCode = code;
            user.PasswordResetCodeExpiry = DateTime.UtcNow.AddMinutes(10);
            user.LastResetCodeSentAt = DateTime.UtcNow;

            await userManager.UpdateAsync(user);

            // 📧 Send email
            await emailService.SendEmailAsync(
                user.Email,
                "Resend Email Reset Code",
                $@"
        <h3>Reset Password</h3>
        <p>Your new Reset code is:</p>
        <h2>{code}</h2>
        <p>This code expires in 10 minutes.</p>
        "
            );

            return Ok(new
            {
                Message = "Reset Password code resent successfully"
            });
        }

    }


}

