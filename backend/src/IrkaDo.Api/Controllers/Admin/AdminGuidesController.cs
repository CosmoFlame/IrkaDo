using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features.Admin;
using IrkaDo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers.Admin;

[Route("api/v1/admin/guides")]
public class AdminGuidesController : AdminControllerBase
{
    private readonly IAppDbContext _db;
    private readonly IFileStorageService _storage;

    public AdminGuidesController(IAppDbContext db, IFileStorageService storage)
    {
        _db = db;
        _storage = storage;
    }

    [HttpGet]
    public async Task<ActionResult<AdminGuideListItemDto[]>> GetAll(CancellationToken ct)
    {
        var items = await _db.TravelGuides.AsNoTracking()
            .OrderByDescending(g => g.UpdatedAt ?? g.CreatedAt)
            .Select(g => new AdminGuideListItemDto(
                g.Id, g.Slug, g.Title, g.Country, g.Continent, g.IsPremium, g.PriceAmount,
                g.PriceCurrency, g.IsPublished, g.IsFeatured,
                g.CoverImage != null ? g.CoverImage.Url : null,
                g.Files.Count, g.UpdatedAt))
            .ToArrayAsync(ct);

        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminGuideDetailDto>> Get(Guid id, CancellationToken ct)
    {
        var guide = await QueryDetailAsync(id, ct);
        return guide is null ? NotFound() : Ok(guide);
    }

    private Task<AdminGuideDetailDto?> QueryDetailAsync(Guid id, CancellationToken ct) =>
        _db.TravelGuides.AsNoTracking()
            .Where(g => g.Id == id)
            .Select(g => new AdminGuideDetailDto(
                g.Id, g.Slug, g.Title, g.TitleEn, g.Country, g.CountryEn, g.City, g.CityEn,
                g.Continent, g.ContinentEn, g.Description, g.DescriptionEn, g.WhatsIncluded, g.WhatsIncludedEn,
                g.DurationDays, g.Difficulty, g.IsPremium, g.PriceAmount, g.PriceCurrency,
                g.IsPublished, g.IsFeatured, g.LastUpdatedAt, g.CoverImageId,
                g.CoverImage != null ? g.CoverImage.Url : null,
                g.PreviewImages.Select(p => p.Id).ToArray(),
                g.Files.Select(f => new AdminGuideFileDto(f.Id, f.FileName, f.SizeBytes)).ToArray(),
                g.MetaTitle, g.MetaTitleEn, g.MetaDescription, g.MetaDescriptionEn, g.OgImageUrl))
            .FirstOrDefaultAsync(ct)!;

    [HttpPost]
    public async Task<ActionResult<AdminGuideDetailDto>> Create([FromBody] AdminGuideUpsertDto dto, CancellationToken ct)
    {
        if (await _db.TravelGuides.AnyAsync(g => g.Slug == dto.Slug, ct))
            return Conflict($"A guide with slug '{dto.Slug}' already exists.");

        var validationError = await ValidateReferencesAsync(dto, ct);
        if (validationError is not null)
            return BadRequest(validationError);

        var guide = new TravelGuide { LastUpdatedAt = DateTimeOffset.UtcNow };
        Apply(guide, dto);
        guide.PreviewImages = await LoadPreviewImagesAsync(dto.PreviewImageIds, ct);

        _db.TravelGuides.Add(guide);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(Get), new { id = guide.Id }, (await QueryDetailAsync(guide.Id, ct))!);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AdminGuideDetailDto>> Update(Guid id, [FromBody] AdminGuideUpsertDto dto, CancellationToken ct)
    {
        var guide = await _db.TravelGuides.Include(g => g.PreviewImages).FirstOrDefaultAsync(g => g.Id == id, ct);
        if (guide is null)
            return NotFound();

        if (await _db.TravelGuides.AnyAsync(g => g.Slug == dto.Slug && g.Id != id, ct))
            return Conflict($"A guide with slug '{dto.Slug}' already exists.");

        var validationError = await ValidateReferencesAsync(dto, ct);
        if (validationError is not null)
            return BadRequest(validationError);

        Apply(guide, dto);
        guide.PreviewImages = await LoadPreviewImagesAsync(dto.PreviewImageIds, ct);
        guide.LastUpdatedAt = DateTimeOffset.UtcNow;
        guide.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Ok((await QueryDetailAsync(guide.Id, ct))!);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var guide = await _db.TravelGuides.FirstOrDefaultAsync(g => g.Id == id, ct);
        if (guide is null)
            return NotFound();

        _db.TravelGuides.Remove(guide);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    // --- Guide files (downloadable PDFs, stored outside wwwroot) ---

    [HttpPost("{id:guid}/files")]
    [RequestSizeLimit(100 * 1024 * 1024)]
    public async Task<ActionResult<AdminGuideFileDto>> UploadFile(Guid id, IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file was uploaded.");

        var guide = await _db.TravelGuides.FirstOrDefaultAsync(g => g.Id == id, ct);
        if (guide is null)
            return NotFound();

        await using var stream = file.OpenReadStream();
        var stored = await _storage.SaveGuideFileAsync(stream, file.FileName, file.ContentType, ct);

        var guideFile = new GuideFile
        {
            TravelGuideId = id,
            StorageKey = stored.StorageKey,
            FileName = file.FileName,
            SizeBytes = stored.SizeBytes
        };
        _db.GuideFiles.Add(guideFile);
        await _db.SaveChangesAsync(ct);

        return Ok(new AdminGuideFileDto(guideFile.Id, guideFile.FileName, guideFile.SizeBytes));
    }

    [HttpDelete("{id:guid}/files/{fileId:guid}")]
    public async Task<IActionResult> DeleteFile(Guid id, Guid fileId, CancellationToken ct)
    {
        var file = await _db.GuideFiles.FirstOrDefaultAsync(f => f.Id == fileId && f.TravelGuideId == id, ct);
        if (file is null)
            return NotFound();

        _db.GuideFiles.Remove(file);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static void Apply(TravelGuide guide, AdminGuideUpsertDto dto)
    {
        guide.Title = dto.Title;
        guide.TitleEn = dto.TitleEn;
        guide.Slug = dto.Slug;
        guide.Country = dto.Country;
        guide.CountryEn = dto.CountryEn;
        guide.City = dto.City;
        guide.CityEn = dto.CityEn;
        guide.Continent = dto.Continent;
        guide.ContinentEn = dto.ContinentEn;
        guide.Description = dto.Description;
        guide.DescriptionEn = dto.DescriptionEn;
        guide.WhatsIncluded = dto.WhatsIncluded;
        guide.WhatsIncludedEn = dto.WhatsIncludedEn;
        guide.DurationDays = dto.DurationDays;
        guide.Difficulty = dto.Difficulty;
        guide.IsPremium = dto.IsPremium;
        guide.PriceAmount = dto.IsPremium ? dto.PriceAmount : null;
        guide.PriceCurrency = dto.PriceCurrency;
        guide.IsPublished = dto.IsPublished;
        guide.IsFeatured = dto.IsFeatured;
        guide.CoverImageId = dto.CoverImageId;
        guide.MetaTitle = dto.MetaTitle;
        guide.MetaTitleEn = dto.MetaTitleEn;
        guide.MetaDescription = dto.MetaDescription;
        guide.MetaDescriptionEn = dto.MetaDescriptionEn;
        guide.OgImageUrl = dto.OgImageUrl;
    }

    private async Task<string?> ValidateReferencesAsync(AdminGuideUpsertDto dto, CancellationToken ct)
    {
        if (!await _db.MediaAssets.AnyAsync(m => m.Id == dto.CoverImageId, ct))
            return "The selected cover image does not exist.";
        if (dto.IsPremium && dto.PriceAmount is null or <= 0)
            return "Premium guides require a price greater than zero.";
        return null;
    }

    private async Task<List<MediaAsset>> LoadPreviewImagesAsync(Guid[] ids, CancellationToken ct)
    {
        if (ids.Length == 0)
            return new List<MediaAsset>();
        return await _db.MediaAssets.Where(m => ids.Contains(m.Id)).ToListAsync(ct);
    }
}
