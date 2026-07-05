using IrkaDo.Application.Common.Interfaces;
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
    public async Task<ActionResult<NewsArticleSummaryDto[]>> GetAll(
        [FromQuery] string? category, [FromQuery] int page = 1, [FromQuery] int pageSize = 12,
        CancellationToken cancellationToken = default)
    {
        var query = _db.NewsArticles.AsNoTracking().Where(a => a.IsPublished);

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(a => a.Category != null && a.Category.Slug == category);
        }

        var items = await query
            .OrderByDescending(a => a.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new NewsArticleSummaryDto(
                a.Slug, a.Title, a.Excerpt,
                a.CoverImage != null ? a.CoverImage.Url : null,
                a.PublishedAt, a.ReadingTimeMinutes,
                a.Category != null ? a.Category.Name : null))
            .ToArrayAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<NewsArticleDetailDto>> GetBySlug(
        string slug, CancellationToken cancellationToken = default)
    {
        var article = await _db.NewsArticles.AsNoTracking()
            .Where(a => a.IsPublished && a.Slug == slug)
            .Select(a => new NewsArticleDetailDto(
                a.Slug, a.Title, a.Content,
                a.CoverImage != null ? a.CoverImage.Url : null,
                a.PublishedAt, a.ReadingTimeMinutes,
                a.Category != null ? a.Category.Name : null,
                a.Tags.Select(t => t.Name).ToArray(),
                a.MetaTitle, a.MetaDescription, a.OgImageUrl))
            .FirstOrDefaultAsync(cancellationToken);

        return article is null ? NotFound() : Ok(article);
    }
}
