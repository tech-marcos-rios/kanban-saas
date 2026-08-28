using Kanban.Domain.Entities;

namespace Kanban.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    DateTime AccessTokenExpiresAt();
    DateTime RefreshTokenExpiresAt();
}
