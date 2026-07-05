using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers;

public record DownloadRequestDto(string? Email);

[ApiController]
[Route("api/v1/guides/{slug}")]
public class GuideDownloadsController : ControllerBase
{
    private readonly IAppDbContext _db;
    private readonly IFileStorageService _storage;

    public GuideDownloadsController(IAppDbContext db, IFileStorageService storage)
    {
        _db = db;
        _storage = storage;
    }

    [HttpPost("download")]
    [EnableRateLimiting("download")]
    public async Task<IActionResult> Download(
        string slug, [FromBody] DownloadRequestDto request, CancellationToken cancellationToken = default)
    {
        var guide = await _db.TravelGuides
            .Include(g => g.Files)
            .FirstOrDefaultAsync(g => g.IsPublished && g.Slug == slug, cancellationToken);

        if (guide is null)
            return NotFound();

        if (guide.IsPremium)
            return BadRequest("This guide requires purchase. Use the checkout endpoint instead.");

        var file = guide.Files.FirstOrDefault();
        if (file is null)
            return NotFound("No downloadable file is attached to this guide yet.");

        _db.DownloadLogs.Add(new DownloadLog
        {
            TravelGuideId = guide.Id,
            Email = request.Email,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"
        });
        await _db.SaveChangesAsync(cancellationToken);

        var downloadUrl = await _storage.GetSignedDownloadUrlAsync(
            file.StorageKey, file.FileName, TimeSpan.FromMinutes(10), cancellationToken);

        return Ok(new { downloadUrl, fileName = file.FileName });
    }
}
