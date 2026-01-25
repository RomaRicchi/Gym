namespace Gym.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string html, CancellationToken ct = default);
}
