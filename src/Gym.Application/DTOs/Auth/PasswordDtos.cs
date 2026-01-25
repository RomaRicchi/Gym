namespace Gym.Application.DTOs.Auth;

public class PasswordUpdateRequest
{
    public string Actual { get; set; } = string.Empty;
    public string Nueva { get; set; } = string.Empty;
}

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
