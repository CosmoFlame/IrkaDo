using System.Security.Cryptography;
using System.Text;
using IrkaDo.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace IrkaDo.Infrastructure.Storage;

/// <summary>
/// Signs storageKey+fileName+expiry into a compact, tamper-proof token so guide files can be
/// served through a time-limited link without requiring user login.
/// </summary>
public class HmacDownloadTokenSigner : IDownloadTokenSigner
{
    private const string DelimiterEscape = "";

    private readonly byte[] _key;

    public HmacDownloadTokenSigner(IOptions<LocalStorageOptions> options)
    {
        _key = Encoding.UTF8.GetBytes(options.Value.SigningKey);
    }

    public string CreateToken(string storageKey, string fileName, TimeSpan expiry)
    {
        var expiresAtUnix = DateTimeOffset.UtcNow.Add(expiry).ToUnixTimeSeconds();
        var payload = string.Join(DelimiterEscape, storageKey, fileName, expiresAtUnix.ToString());
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var signatureBytes = HMACSHA256.HashData(_key, payloadBytes);

        return $"{Base64UrlEncode(payloadBytes)}.{Base64UrlEncode(signatureBytes)}";
    }

    public bool TryValidate(string token, out string storageKey, out string fileName)
    {
        storageKey = string.Empty;
        fileName = string.Empty;

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

        var payload = Encoding.UTF8.GetString(payloadBytes);
        var segments = payload.Split(DelimiterEscape);
        if (segments.Length != 3)
            return false;

        if (!long.TryParse(segments[2], out var expiresAtUnix))
            return false;

        if (DateTimeOffset.FromUnixTimeSeconds(expiresAtUnix) < DateTimeOffset.UtcNow)
            return false;

        storageKey = segments[0];
        fileName = segments[1];
        return true;
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
