namespace IrkaDo.Infrastructure.Auth;

public class AdminOptions
{
    /// <summary>Shared admin password. Override via env/user-secrets outside development.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>HMAC key used to sign session tokens. Change from the dev default in real use.</summary>
    public string TokenSigningKey { get; set; } = "dev-admin-signing-key-change-me";

    /// <summary>Session token lifetime in hours.</summary>
    public int TokenLifetimeHours { get; set; } = 12;
}
