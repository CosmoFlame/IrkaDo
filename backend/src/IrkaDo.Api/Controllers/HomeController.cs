using IrkaDo.Api.Localization;
using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features;
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
        var en = Request.IsEnglish();

        var hero = await _db.HomeSections.AsNoTracking()
            .Where(s => s.Type == HomeSectionType.Hero)
            .Select(s => new HeroDto(
                en && s.HeadlineEn != null ? s.HeadlineEn : s.Headline,
                en && s.BodyEn != null ? s.BodyEn : s.Body,
                s.BackgroundMedia != null ? s.BackgroundMedia.Url : null))
            .FirstOrDefaultAsync(cancellationToken) ?? new HeroDto(string.Empty, string.Empty, null);

        var about = await _db.HomeSections.AsNoTracking()
            .Where(s => s.Type == HomeSectionType.About)
            .Select(s => new AboutDto(
                en && s.HeadlineEn != null ? s.HeadlineEn : s.Headline,
                en && s.BodyEn != null ? s.BodyEn : s.Body))
            .FirstOrDefaultAsync(cancellationToken) ?? new AboutDto(string.Empty, string.Empty);

        var contact = await _db.HomeSections.AsNoTracking()
            .Where(s => s.Type == HomeSectionType.Contact)
            .Select(s => new ContactDto(
                en && s.HeadlineEn != null ? s.HeadlineEn : s.Headline,
                en && s.BodyEn != null ? s.BodyEn : s.Body,
                s.ContactEmail))
            .FirstOrDefaultAsync(cancellationToken) ?? new ContactDto(string.Empty, string.Empty, null);

        var socialLinks = await _db.SocialLinks.AsNoTracking()
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new SocialLinkDto(
                s.Platform.ToString(), s.Url,
                en && s.DescriptionEn != null ? s.DescriptionEn : s.Description,
                s.FollowerCount))
            .ToArrayAsync(cancellationToken);

        var collaborations = await _db.Collaborations.AsNoTracking()
            .Where(c => c.IsPublished)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CollaborationDto(
                c.BrandName,
                en && c.DescriptionEn != null ? c.DescriptionEn : c.Description,
                en && c.TestimonialEn != null ? c.TestimonialEn : c.Testimonial,
                c.CoverImage != null ? c.CoverImage.Url : null,
                c.CoverImage != null ? (en && c.CoverImage.AltTextEn != null ? c.CoverImage.AltTextEn : c.CoverImage.AltText) : null,
                c.Links.OrderBy(l => l.DisplayOrder)
                    .Select(l => new LinkDto(l.Url, en && l.TitleEn != null ? l.TitleEn : l.Title))
                    .ToArray()))
            .ToArrayAsync(cancellationToken);

        var featuredGuides = await _db.TravelGuides.AsNoTracking()
            .Where(g => g.IsPublished && g.IsFeatured)
            .OrderByDescending(g => g.CreatedAt)
            .Take(6)
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

        var latestNews = await _db.NewsArticles.AsNoTracking()
            .Where(a => a.IsPublished)
            .OrderByDescending(a => a.PublishedAt)
            .Take(3)
            .Select(a => new NewsArticleSummaryDto(
                a.Slug,
                en && a.TitleEn != null ? a.TitleEn : a.Title,
                en && a.ExcerptEn != null ? a.ExcerptEn : a.Excerpt,
                a.CoverImage != null ? a.CoverImage.Url : null,
                a.CoverImage != null ? (en && a.CoverImage.AltTextEn != null ? a.CoverImage.AltTextEn : a.CoverImage.AltText) : null,
                a.PublishedAt, a.ReadingTimeMinutes,
                a.Category != null ? (en && a.Category.NameEn != null ? a.Category.NameEn : a.Category.Name) : null))
            .ToArrayAsync(cancellationToken);

        return Ok(new HomePageDto(
            hero, about, contact, socialLinks, collaborations, featuredGuides, latestNews));
    }
}
