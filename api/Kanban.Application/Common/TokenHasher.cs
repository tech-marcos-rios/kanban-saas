using System.Security.Cryptography;
using System.Text;

namespace Kanban.Application.Common;

/// <summary>
/// El refresh token es un secreto de larga vida (7 días) equivalente a una contraseña:
/// si se filtra un dump de la DB, guardarlo en texto plano deja generar access tokens sin
/// necesitar la contraseña. Se guarda su hash SHA-256 (alcanza, no hace falta BCrypt acá
/// porque el token ya es aleatorio de alta entropía, no algo adivinable por fuerza bruta).
/// </summary>
public static class TokenHasher
{
    public static string Hash(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
