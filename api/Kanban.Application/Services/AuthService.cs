using Kanban.Application.Common;
using Kanban.Application.DTOs.Auth;
using Kanban.Application.Interfaces;
using Kanban.Domain.Entities;

namespace Kanban.Application.Services;

public class AuthService
{
    private const int MinPasswordLength = 8;
    private const int MaxPasswordLength = 100;

    // Hash bcrypt "señuelo" contra el que verificar cuando el usuario no existe, para que
    // el tiempo de respuesta del login no delate si un email está o no registrado.
    private static readonly string DummyPasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString());

    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly IJwtService _jwt;

    public AuthService(IUserRepository users, IUnitOfWork uow, IJwtService jwt)
    {
        _users = users;
        _uow = uow;
        _jwt = jwt;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var passwordResult = ValidatePassword(request.Password);
        if (passwordResult.IsFailure)
            return Result.Failure<AuthResponse>(passwordResult.Error!);

        if (!EmailNormalizer.IsValidFormat(request.Email))
            return Result.Failure<AuthResponse>("El email no tiene un formato válido.");

        var email = EmailNormalizer.Normalize(request.Email);
        if (await _users.ExistsByEmailAsync(email, ct))
            return Result.Failure<AuthResponse>("Ya existe una cuenta con ese email.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = User.Create(request.Name, email, passwordHash, Role.WellKnownIds.UserRoleId);

        var refreshToken = _jwt.GenerateRefreshToken();
        user.SetRefreshToken(TokenHasher.Hash(refreshToken), _jwt.RefreshTokenExpiresAt());

        await _users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        // Recargamos para obtener la navegación Role (necesaria en BuildResponse)
        var saved = await _users.GetByIdAsync(user.Id, ct);
        var accessToken = _jwt.GenerateAccessToken(saved!);
        return Result.Success(BuildResponse(saved!, accessToken, refreshToken));
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByEmailAsync(EmailNormalizer.Normalize(request.Email), ct);

        // Se verifica siempre, incluso con usuario null (contra un hash señuelo), para que
        // el tiempo de respuesta no varíe según si el email existe o no.
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user?.PasswordHash ?? DummyPasswordHash);
        if (user is null || !isPasswordValid)
            return Result.Failure<AuthResponse>("Credenciales incorrectas.");

        var accessToken = _jwt.GenerateAccessToken(user);
        var refreshToken = _jwt.GenerateRefreshToken();
        user.SetRefreshToken(TokenHasher.Hash(refreshToken), _jwt.RefreshTokenExpiresAt());

        _users.Update(user);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(BuildResponse(user, accessToken, refreshToken));
    }

    public async Task<Result<AuthResponse>> RefreshAsync(RefreshTokenRequest request, CancellationToken ct = default)
    {
        var hashedToken = TokenHasher.Hash(request.RefreshToken);
        var user = await _users.GetByRefreshTokenAsync(hashedToken, ct);

        if (user is null || !user.IsRefreshTokenValid(hashedToken))
            return Result.Failure<AuthResponse>("Refresh token inválido o expirado.");

        var accessToken = _jwt.GenerateAccessToken(user);
        var newRefreshToken = _jwt.GenerateRefreshToken();
        user.SetRefreshToken(TokenHasher.Hash(newRefreshToken), _jwt.RefreshTokenExpiresAt());

        _users.Update(user);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(BuildResponse(user, accessToken, newRefreshToken));
    }

    public async Task<Result> LogoutAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(userId, ct);
        if (user is null) return Result.Failure("Usuario no encontrado.");

        user.RevokeRefreshToken();
        _users.Update(user);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }

    private AuthResponse BuildResponse(User user, string accessToken, string refreshToken) =>
        new(accessToken, refreshToken, _jwt.AccessTokenExpiresAt(), user.Name, user.Email, user.Role.Name);

    private static Result<string> ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < MinPasswordLength)
            return Result.Failure<string>($"La contraseña debe tener al menos {MinPasswordLength} caracteres.");
        if (password.Length > MaxPasswordLength)
            return Result.Failure<string>($"La contraseña no puede superar los {MaxPasswordLength} caracteres.");
        if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit))
            return Result.Failure<string>("La contraseña debe incluir al menos una letra y un número.");

        return Result.Success(password);
    }
}
