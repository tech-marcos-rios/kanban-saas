using System.Net.Mail;

namespace Kanban.Application.Common;

/// <summary>
/// Normaliza emails antes de guardarlos o compararlos, para que "Test@x.com" y "test@x.com"
/// no terminen siendo cuentas distintas ni rompan una invitación por casing.
/// </summary>
public static class EmailNormalizer
{
    public static string Normalize(string email) => email.Trim().ToLowerInvariant();

    /// <summary>
    /// Chequeo de formato básico — nada validaba esto antes (ni register ni invitación
    /// de miembros), así que se podían crear cuentas con "no-es-un-email" y quedaban
    /// inaccesibles para siempre. Se usa MailAddress en vez de una regex a mano porque
    /// cubre los casos raros (comillas, subdominios, +tags) sin reinventar RFC 5322.
    /// </summary>
    public static bool IsValidFormat(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            return new MailAddress(email.Trim()).Address == email.Trim();
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
