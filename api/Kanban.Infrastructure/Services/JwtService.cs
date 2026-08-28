using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Kanban.Application.Interfaces;
using Kanban.Domain.Entities;

namespace Kanban.Infrastructure.Services;

/// <summary>
/// Access token JWT (HMAC-SHA256, vida corta) + refresh token opaco (CSPRNG, vida larga,
/// almacenado en BD para poder revocarlo en logout). Todos los parámetros salen de config,
/// nunca hardcodeados.
/// </summary>
public class JwtService : IJwtService
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenMinutes;
    private readonly int _refreshTokenDays;

    public JwtService(IConfiguration config)
    {
        _key = config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key no configurada.");
        if (_key.Length < 32)
            throw new InvalidOperationException("Jwt:Key debe tener al menos 32 caracteres para HMAC-SHA256.");
        _issuer = config["Jwt:Issuer"] ?? "kanban-api";
        _audience = config["Jwt:Audience"] ?? "kanban-web";
        _accessTokenMinutes = int.TryParse(config["Jwt:AccessTokenMinutes"], out var m) ? m : 60;
        _refreshTokenDays = int.TryParse(config["Jwt:RefreshTokenDays"], out var d) ? d : 7;
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: AccessTokenExpiresAt(),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <remarks>RandomNumberGenerator (CSPRNG), no Random ni Guid — 64 bytes = 512 bits de entropía.</remarks>
    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public DateTime AccessTokenExpiresAt() => DateTime.UtcNow.AddMinutes(_accessTokenMinutes);
    public DateTime RefreshTokenExpiresAt() => DateTime.UtcNow.AddDays(_refreshTokenDays);
}
