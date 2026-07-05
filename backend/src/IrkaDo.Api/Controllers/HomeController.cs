using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features.Guides;
using IrkaDo.Application.Features.Home;
using IrkaDo.Application.Features.News;
using IrkaDo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers;

[ApiController]
[Route("api/v1/home")]
public class HomeController : ControllerBase
{
    private readonly IAppDbContext _db;

    public HomeController(IAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<HomePageDto>> Get(CancellationToken cancellationToken = default)
    {
        var hero = await _db.HomeSections.AsNoTracking()
            .Where(s => s.Type == HomeSectionType.Hero)
            .Select(s => new HeroDto(s.Headline, s.Body, s.BackgroundMedia != null ? s.BackgroundMedia.Url : null))
            .FirstOrDefaultAsync(cancellationToken) ?? new HeroDto(string.Empty, string.Empty, null);

        var about = await _db.HomeSections.AsNoTracking()
            .Where(s => s.Type == HomeSectionType.About)
            .Select(s => new AboutDto(s.Headline, s.Body))
            .FirstOrDefaultAsync(cancellationToken) ?? new AboutDto(string.Empty, string.Empty);

        var contact = await _db.HomeSections.AsNoTracking()
            .Where(s => s.Type == HomeSectionType.Contact)
            .Select(s => new ContactDto(s.Headline, s.Body))
            .FirstOrDefaultAsync(cancellationToken) ?? new ContactDto(string.Empty, string.Empty);

        var highlights = await _db.TravelHighlights.AsNoTracking()
            .Where(h => h.IsPublished)
            .OrderBy(h => h.DisplayOrder)
            .Select(h => new TravelHighlightDto(h.Destination, h.Caption, h.Image != null ? h.Image.Url : null))
            .ToArrayAsync(cancellationToken);

        var socialLinks = await _db.SocialLinks.AsNoTracking()
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new SocialLinkDto(s.Platform.ToString(), s.Url, s.Description, s.FollowerCount))
            .ToArrayAsync(cancellationToken);

        var collaborations = await _db.Collaborations.AsNoTracking()
            .Where(c => c.IsPublished)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CollaborationDto(
                c.BrandName, c.Description, c.Testimonial,
                c.Logo != null ? c.Logo.Url : null,
                c.CampaignImages.Select(m => m.Url).ToArray()))
            .ToArrayAsync(cancellationToken);

        var featuredGuides = await _db.TravelGuides.AsNoTracking()
            .Where(g => g.IsPublished && g.IsFeatured)
            .OrderByDescending(g => g.CreatedAt)
            .Take(6)
            .Select(g => new TravelGuideSummaryDto(
                g.Slug, g.Title, g.Country, g.City, g.Continent, g.DurationDays,
                g.Difficulty != null ? g.Difficulty.ToString() : null,
                g.IsPremium, g.PriceAmount, g.PriceCurrency,
                g.CoverImage != null ? g.CoverImage.Url : null))
            .ToArrayAsync(cancellationToken);

        var latestNews = await _db.NewsArticles.AsNoTracking()
            .Where(a => a.IsPublished)
            .OrderByDescending(a => a.PublishedAt)
            .Take(3)
            .Select(a => new NewsArticleSummaryDto(
                a.Slug, a.Title, a.Excerpt,
                a.CoverImage != null ? a.CoverImage.Url : null,
                a.PublishedAt, a.ReadingTimeMinutes,
                a.Category != null ? a.Category.Name : null))
            .ToArrayAsync(cancellationToken);

        return Ok(new HomePageDto(
            hero, about, contact, highlights, socialLinks, collaborations, featuredGuides, latestNews));
    }
}
