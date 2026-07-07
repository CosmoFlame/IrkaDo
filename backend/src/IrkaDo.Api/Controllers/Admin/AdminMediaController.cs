using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features.Admin;
using IrkaDo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers.Admin;

[Route("api/v1/admin/media")]
public class AdminMediaController : AdminControllerBase
{
    private readonly IAppDbContext _db;
    private readonly IFileStorageService _storage;

    public AdminMediaController(IAppDbContext db, IFileStorageService storage)
    {
        _db = db;
        _storage = storage;
    }

    [HttpGet]
    public async Task<ActionResult<AdminMediaDto[]>> GetAll(CancellationToken ct)
    {
        var items = await _db.MediaAssets.AsNoTracking()
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new AdminMediaDto(m.Id, m.Url, m.Type, m.AltText, m.AltTextEn, m.Width, m.Height, m.CreatedAt))
            .ToArrayAsync(ct);
        return Ok(items);
    }

    [HttpPost]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<ActionResult<AdminMediaDto>> Upload(IFormFile file, [FromForm] string? altText, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file was uploaded.");

        await using var stream = file.OpenReadStream();
        var stored = await _storage.SaveAsync(stream, file.FileName, file.ContentType, ct);

        var asset = new MediaAsset
        {
            Url = stored.PublicUrl,
            Type = ResolveType(file.ContentType),
            AltText = altText
        };
        _db.MediaAssets.Add(asset);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetAll), new { id = asset.Id },
            new AdminMediaDto(asset.Id, asset.Url, asset.Type, asset.AltText, asset.AltTextEn, asset.Width, asset.Height, asset.CreatedAt));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AdminMediaDto>> Update(Guid id, [FromBody] AdminMediaUpdateDto dto, CancellationToken ct)
    {
        var asset = await _db.MediaAssets.FirstOrDefaultAsync(m => m.Id == id, ct);
        if (asset is null)
            return NotFound();

        asset.AltText = dto.AltText;
        asset.AltTextEn = dto.AltTextEn;
        asset.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Ok(new AdminMediaDto(asset.Id, asset.Url, asset.Type, asset.AltText, asset.AltTextEn, asset.Width, asset.Height, asset.CreatedAt));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var asset = await _db.MediaAssets.FirstOrDefaultAsync(m => m.Id == id, ct);
        if (asset is null)
            return NotFound();

        // Required image FKs cascade principal→dependent, so deleting an in-use asset would delete
        // its owning article/guide/collaboration/highlight. Refuse instead.
        if (await IsInUseAsync(id, ct))
            return Conflict("This image is still used by content. Remove those references before deleting it.");

        _db.MediaAssets.Remove(asset);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private async Task<bool> IsInUseAsync(Guid id, CancellationToken ct) =>
        await _db.NewsArticles.AnyAsync(a => a.CoverImageId == id, ct) ||
        await _db.TravelGuides.AnyAsync(g => g.CoverImageId == id || g.PreviewImages.Any(p => p.Id == id), ct) ||
        await _db.Collaborations.AnyAsync(c => c.LogoId == id || c.CampaignImages.Any(m => m.Id == id), ct) ||
        await _db.TravelHighlights.AnyAsync(h => h.ImageId == id, ct) ||
        await _db.HomeSections.AnyAsync(s => s.BackgroundMediaId == id, ct);

    private static MediaAssetType ResolveType(string? contentType) => contentType switch
    {
        not null when contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) => MediaAssetType.Image,
        not null when contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase) => MediaAssetType.Video,
        _ => MediaAssetType.Document
    };
}
