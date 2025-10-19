using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Api.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string html);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string html)
        {
            try
            {
                using var smtp = new SmtpClient(_config["Smtp:Host"])
                {
                    Port = int.Parse(_config["Smtp:Port"]),
                    Credentials = new NetworkCredential(
                        _config["Smtp:User"],
                        _config["Smtp:Pass"]
                    ),
                    EnableSsl = true
                };

                using var mail = new MailMessage
                {
                    From = new MailAddress(_config["Smtp:User"], "GYM SYSTEM"),
                    Subject = subject,
                    Body = html,
                    IsBodyHtml = true
                };

                mail.To.Add(to);

                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al enviar correo: {ex.Message}");
                throw; // repropaga el error para depuración
            }
        }
    }
}
