using System.Security.Cryptography;
using System.Text;
using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace IrkaDo.Api.Controllers.Admin;

public record AdminLoginRequest(string Password);

public record AdminLoginResponse(string Token, DateTimeOffset ExpiresAt);

[ApiController]
[Route("api/v1/admin/login")]
public class AdminAuthController : ControllerBase
{
    private readonly AdminOptions _options;
    private readonly IAdminTokenService _tokenService;

    public AdminAuthController(IOptions<AdminOptions> options, IAdminTokenService tokenService)
    {
        _options = options.Value;
        _tokenService = tokenService;
    }

    [HttpPost]
    [EnableRateLimiting("login")]
    public ActionResult<AdminLoginResponse> Login([FromBody] AdminLoginRequest request)
    {
        if (string.IsNullOrEmpty(_options.Password) || !IsPasswordCorrect(request.Password))
        {
            return Unauthorized();
        }

        var lifetime = TimeSpan.FromHours(_options.TokenLifetimeHours);
        var token = _tokenService.CreateToken(lifetime);
        return Ok(new AdminLoginResponse(token, DateTimeOffset.UtcNow.Add(lifetime)));
    }

    private bool IsPasswordCorrect(string? provided)
    {
        var providedBytes = Encoding.UTF8.GetBytes(provided ?? string.Empty);
        var expectedBytes = Encoding.UTF8.GetBytes(_options.Password);
        return CryptographicOperations.FixedTimeEquals(providedBytes, expectedBytes);
    }
}
