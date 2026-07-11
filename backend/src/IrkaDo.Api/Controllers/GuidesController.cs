using IrkaDo.Api.Localization;
using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features;
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
        [FromQuery] string? country, [FromQuery] string? type,
        CancellationToken cancellationToken = default)
    {
        var en = Request.IsEnglish();
        var query = _db.TravelGuides.AsNoTracking().Where(g => g.IsPublished);

        // Filter against the same (localized) value the frontend displayed and sent back.
        if (!string.IsNullOrWhiteSpace(country))
            query = query.Where(g => (en && g.CountryEn != null ? g.CountryEn : g.Country) == country);

        if (string.Equals(type, "free", StringComparison.OrdinalIgnoreCase))
            query = query.Where(g => !g.IsPremium);
        else if (string.Equals(type, "premium", StringComparison.OrdinalIgnoreCase))
            query = query.Where(g => g.IsPremium);

        var items = await query
            .OrderByDescending(g => g.IsFeatured)
            .ThenByDescending(g => g.CreatedAt)
            .Select(g => new TravelGuideSummaryDto(
                g.Slug,
                en && g.TitleEn != null ? g.TitleEn : g.Title,
                en && g.CountryEn != null ? g.CountryEn : g.Country,
                en && g.CityEn != null ? g.CityEn : g.City,
                g.DurationDays,
                g.Difficulty != null ? g.Difficulty.ToString() : null,
                g.IsPremium, g.PriceAmount, g.PriceCurrency,
                g.CoverImage != null ? g.CoverImage.Url : null,
                g.CoverImage != null ? (en && g.CoverImage.AltTextEn != null ? g.CoverImage.AltTextEn : g.CoverImage.AltText) : null))
            .ToArrayAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<TravelGuideDetailDto>> GetBySlug(
        string slug, CancellationToken cancellationToken = default)
    {
        var en = Request.IsEnglish();
        var guide = await _db.TravelGuides.AsNoTracking()
            .Where(g => g.IsPublished && g.Slug == slug)
            .Select(g => new TravelGuideDetailDto(
                g.Slug,
                en && g.TitleEn != null ? g.TitleEn : g.Title,
                en && g.CountryEn != null ? g.CountryEn : g.Country,
                en && g.CityEn != null ? g.CityEn : g.City,
                en && g.DescriptionEn != null ? g.DescriptionEn : g.Description,
                en && g.WhatsIncludedEn != null ? g.WhatsIncludedEn : g.WhatsIncluded,
                g.DurationDays, g.Difficulty != null ? g.Difficulty.ToString() : null,
                g.IsPremium, g.PriceAmount, g.PriceCurrency,
                g.CoverImage != null ? g.CoverImage.Url : null,
                g.CoverImage != null ? (en && g.CoverImage.AltTextEn != null ? g.CoverImage.AltTextEn : g.CoverImage.AltText) : null,
                g.PreviewImages
                    .Select(m => new ImageDto(m.Url, en && m.AltTextEn != null ? m.AltTextEn : m.AltText))
                    .ToArray(),
                g.Links.OrderBy(l => l.DisplayOrder)
                    .Select(l => new LinkDto(l.Url, en && l.TitleEn != null ? l.TitleEn : l.Title))
                    .ToArray(),
                g.Files.Any(),
                g.LastUpdatedAt,
                en && g.MetaTitleEn != null ? g.MetaTitleEn : g.MetaTitle,
                en && g.MetaDescriptionEn != null ? g.MetaDescriptionEn : g.MetaDescription,
                g.OgImageUrl))
            .FirstOrDefaultAsync(cancellationToken);

        return guide is null ? NotFound() : Ok(guide);
    }
}
