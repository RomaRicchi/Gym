using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task SendEmailAsync(string to, string subject, string html)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("El destinatario no puede ser nulo o vacío.", nameof(to));

            if (string.IsNullOrWhiteSpace(subject))
                subject = "(Sin asunto)";

            if (string.IsNullOrWhiteSpace(html))
                html = "(Mensaje vacío)";

            // ⚠️ Validamos valores de configuración
            var host = _config["Smtp:Host"] ?? throw new InvalidOperationException("Falta configurar Smtp:Host");
            var portStr = _config["Smtp:Port"] ?? throw new InvalidOperationException("Falta configurar Smtp:Port");
            var user = _config["Smtp:User"] ?? throw new InvalidOperationException("Falta configurar Smtp:User");
            var pass = _config["Smtp:Pass"] ?? throw new InvalidOperationException("Falta configurar Smtp:Pass");

            if (!int.TryParse(portStr, out int port))
                throw new FormatException("El valor de Smtp:Port no es un número válido.");

            try
            {
                using var smtp = new SmtpClient(host)
                {
                    Port = port,
                    Credentials = new NetworkCredential(user, pass),
                    EnableSsl = true
                };

                using var mail = new MailMessage
                {
                    From = new MailAddress(user, "GYM SYSTEM"),
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
                throw; // repropaga para depurar
            }
        }
    }
}
