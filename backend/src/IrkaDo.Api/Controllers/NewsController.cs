using IrkaDo.Api.Localization;
using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features;
using IrkaDo.Application.Features.News;
using IrkaDo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers;

[ApiController]
[Route("api/v1/news")]
public class NewsController : ControllerBase
{
    private readonly IAppDbContext _db;

    public NewsController(IAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<PagedResult<NewsArticleSummaryDto>>> GetAll(
        [FromQuery] string? category, [FromQuery] int page = 1, [FromQuery] int pageSize = 12,
        CancellationToken cancellationToken = default)
    {
        var en = Request.IsEnglish();
        var query = _db.NewsArticles.AsNoTracking().Where(a => a.IsPublished);

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(a => a.Category != null && a.Category.Slug == category);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new NewsArticleSummaryDto(
                a.Slug,
                en && a.TitleEn != null ? a.TitleEn : a.Title,
                en && a.ExcerptEn != null ? a.ExcerptEn : a.Excerpt,
                a.CoverImage != null ? a.CoverImage.Url : null,
                a.PublishedAt, a.ReadingTimeMinutes,
                a.Category != null ? (en && a.Category.NameEn != null ? a.Category.NameEn : a.Category.Name) : null))
            .ToArrayAsync(cancellationToken);

        return Ok(new PagedResult<NewsArticleSummaryDto>(items, page, pageSize, totalCount));
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<NewsArticleDetailDto>> GetBySlug(
        string slug, CancellationToken cancellationToken = default)
    {
        var en = Request.IsEnglish();
        var article = await _db.NewsArticles.AsNoTracking()
            .Where(a => a.IsPublished && a.Slug == slug)
            .Select(a => new NewsArticleDetailDto(
                a.Slug,
                en && a.TitleEn != null ? a.TitleEn : a.Title,
                en && a.ContentEn != null ? a.ContentEn : a.Content,
                a.CoverImage != null ? a.CoverImage.Url : null,
                a.PublishedAt, a.ReadingTimeMinutes,
                a.Category != null ? (en && a.Category.NameEn != null ? a.Category.NameEn : a.Category.Name) : null,
                a.Tags.Select(t => en && t.NameEn != null ? t.NameEn : t.Name).ToArray(),
                en && a.MetaTitleEn != null ? a.MetaTitleEn : a.MetaTitle,
                en && a.MetaDescriptionEn != null ? a.MetaDescriptionEn : a.MetaDescription,
                a.OgImageUrl))
            .FirstOrDefaultAsync(cancellationToken);

        return article is null ? NotFound() : Ok(article);
    }
}
