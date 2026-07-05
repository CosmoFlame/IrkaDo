using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features.Guides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers;

[ApiController]
[Route("api/v1/guides")]
public class GuidesController : ControllerBase
{
    private readonly IAppDbContext _db;

    public GuidesController(IAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<TravelGuideSummaryDto[]>> GetAll(
        [FromQuery] string? country, [FromQuery] string? continent, [FromQuery] string? type,
        CancellationToken cancellationToken = default)
    {
        var query = _db.TravelGuides.AsNoTracking().Where(g => g.IsPublished);

        if (!string.IsNullOrWhiteSpace(country))
            query = query.Where(g => g.Country == country);

        if (!string.IsNullOrWhiteSpace(continent))
            query = query.Where(g => g.Continent == continent);

        if (string.Equals(type, "free", StringComparison.OrdinalIgnoreCase))
            query = query.Where(g => !g.IsPremium);
        else if (string.Equals(type, "premium", StringComparison.OrdinalIgnoreCase))
            query = query.Where(g => g.IsPremium);

        var items = await query
            .OrderByDescending(g => g.IsFeatured)
            .ThenByDescending(g => g.CreatedAt)
            .Select(g => new TravelGuideSummaryDto(
                g.Slug, g.Title, g.Country, g.City, g.Continent, g.DurationDays,
                g.Difficulty != null ? g.Difficulty.ToString() : null,
                g.IsPremium, g.PriceAmount, g.PriceCurrency,
                g.CoverImage != null ? g.CoverImage.Url : null))
            .ToArrayAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<TravelGuideDetailDto>> GetBySlug(
        string slug, CancellationToken cancellationToken = default)
    {
        var guide = await _db.TravelGuides.AsNoTracking()
            .Where(g => g.IsPublished && g.Slug == slug)
            .Select(g => new TravelGuideDetailDto(
                g.Slug, g.Title, g.Country, g.City, g.Continent, g.Description, g.WhatsIncluded,
                g.DurationDays, g.Difficulty != null ? g.Difficulty.ToString() : null,
                g.IsPremium, g.PriceAmount, g.PriceCurrency,
                g.CoverImage != null ? g.CoverImage.Url : null,
                g.PreviewImages.Select(m => m.Url).ToArray(),
                g.LastUpdatedAt, g.MetaTitle, g.MetaDescription, g.OgImageUrl))
            .FirstOrDefaultAsync(cancellationToken);

        return guide is null ? NotFound() : Ok(guide);
    }
}
