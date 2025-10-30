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

namespace E_commarce_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IConfiguration configuration) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new AppUser { UserName = model.Email, Email = model.Email, FullName = model.FullName };
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "User registered successfully!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized();

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized();

            return Ok(new { Token = GenerateJwtToken(user) });
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


        private string GenerateJwtToken(AppUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes( configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private async Task SendResetEmailAsync(string email, string token)
        {
            var resetLink = $"https://yourfrontend.com/reset-password?email={email}&token={Uri.EscapeDataString(token)}";

            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("youremail@gmail.com", "your-app-password");

                var mail = new MailMessage("youremail@gmail.com", email)
                {
                    Subject = "Password Reset",
                    Body = $"Click the link to reset your password: {resetLink}"
                };

                await client.SendMailAsync(mail);
            }
        }

    }


}

