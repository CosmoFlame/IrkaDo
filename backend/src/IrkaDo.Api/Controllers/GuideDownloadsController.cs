using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers;

public record DownloadRequestDto(string? Email);

[ApiController]
[Route("api/v1/guides/{slug}")]
public class GuideDownloadsController : ControllerBase
{
    private readonly IAppDbContext _db;

    public GuideDownloadsController(IAppDbContext db) => _db = db;

    [HttpPost("download")]
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

        // Storage-signed URL generation is added when object storage is wired up (Phase 1).
        return Ok(new { downloadUrl = file.StorageKey, fileName = file.FileName });
    }
}
