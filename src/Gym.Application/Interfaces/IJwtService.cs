using Gym.Domain.Entities;

namespace Gym.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(Usuario usuario);
    int? ValidateToken(string token);
}
