namespace Kanban.Application.Common;

/// <summary>
/// Normaliza emails antes de guardarlos o compararlos, para que "Test@x.com" y "test@x.com"
/// no terminen siendo cuentas distintas ni rompan una invitación por casing.
/// </summary>
public static class EmailNormalizer
{
    public static string Normalize(string email) => email.Trim().ToLowerInvariant();
}
