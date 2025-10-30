using System.Net;
using System.Net.Mail;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlContent);
}

namespace E_commarce_Backend.Services
{

    public class SmtpEmailService(IConfiguration configuration) : IEmailService
    {

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            var smtp = configuration.GetSection("SmtpSettings");

            using (var client = new SmtpClient())
            {
                client.Host = smtp["Host"];
                client.Port = int.Parse(smtp["Port"]!);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]);

                var message = new MailMessage
                {
                    From = new MailAddress(smtp["FromEmail"], smtp["FromName"]),
                    Subject = subject,
                    Body = htmlContent,
                    IsBodyHtml = true
                };

                message.To.Add(toEmail);

                try
                {
                    await client.SendMailAsync(message);
                }
                catch (SmtpException ex)
                {
                    throw new Exception($"SMTP error: {ex.Message}", ex);
                }
            }
        }

    }
}
