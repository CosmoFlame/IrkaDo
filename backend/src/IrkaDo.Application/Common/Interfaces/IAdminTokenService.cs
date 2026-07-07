namespace IrkaDo.Application.Common.Interfaces;

/// <summary>
/// Issues and validates the signed session token handed out after a successful admin
/// password login. Kept as a seam so a future move to ASP.NET Identity + roles is a
/// swap here rather than a rewrite of the admin controllers.
/// </summary>
public interface IAdminTokenService
{
    string CreateToken(TimeSpan expiry);

    bool TryValidate(string token);
}
