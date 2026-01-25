using System.Net;
using System.Net.Mail;
using Gym.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gym.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string html, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(to))
            throw new ArgumentException("El destinatario no puede ser nulo o vacío.", nameof(to));

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
                From = new MailAddress(user, "GYM SAAS"),
                Subject = subject ?? "(Sin asunto)",
                Body = html ?? "(Mensaje vacío)",
                IsBodyHtml = true
            };

            mail.To.Add(to);

            await smtp.SendMailAsync(mail, ct);
            _logger.LogInformation("Email enviado a {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar correo a {To}", to);
            throw;
        }
    }
}
