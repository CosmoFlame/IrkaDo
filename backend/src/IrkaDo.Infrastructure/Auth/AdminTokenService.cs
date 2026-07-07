using System.Security.Cryptography;
using System.Text;
using IrkaDo.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace IrkaDo.Infrastructure.Auth;

/// <summary>
/// Signs a compact "subject|expiry" payload with HMAC-SHA256, mirroring
/// <see cref="Storage.HmacDownloadTokenSigner"/>. The single admin identity means the token
/// carries no user id — just proof it was issued after a valid password login and hasn't expired.
/// </summary>
public class AdminTokenService : IAdminTokenService
{
    private const string Subject = "admin";
    private const char Delimiter = '|';

    private readonly byte[] _key;

    public AdminTokenService(IOptions<AdminOptions> options)
    {
        _key = Encoding.UTF8.GetBytes(options.Value.TokenSigningKey);
    }

    public string CreateToken(TimeSpan expiry)
    {
        var expiresAtUnix = DateTimeOffset.UtcNow.Add(expiry).ToUnixTimeSeconds();
        var payload = string.Join(Delimiter, Subject, expiresAtUnix.ToString());
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var signatureBytes = HMACSHA256.HashData(_key, payloadBytes);

        return $"{Base64UrlEncode(payloadBytes)}.{Base64UrlEncode(signatureBytes)}";
    }

    public bool TryValidate(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var parts = token.Split('.', 2);
        if (parts.Length != 2)
            return false;

        byte[] payloadBytes;
        byte[] signatureBytes;
        try
        {
            payloadBytes = Base64UrlDecode(parts[0]);
            signatureBytes = Base64UrlDecode(parts[1]);
        }
        catch (FormatException)
        {
            return false;
        }

        var expectedSignature = HMACSHA256.HashData(_key, payloadBytes);
        if (!CryptographicOperations.FixedTimeEquals(signatureBytes, expectedSignature))
            return false;

        var segments = Encoding.UTF8.GetString(payloadBytes).Split(Delimiter);
        if (segments.Length != 2 || segments[0] != Subject)
            return false;

        if (!long.TryParse(segments[1], out var expiresAtUnix))
            return false;

        return DateTimeOffset.FromUnixTimeSeconds(expiresAtUnix) >= DateTimeOffset.UtcNow;
    }

    private static string Base64UrlEncode(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static byte[] Base64UrlDecode(string value)
    {
        var padded = value.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
        }
        return Convert.FromBase64String(padded);
    }
}
