using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features.Admin;
using IrkaDo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers.Admin;

[Route("api/v1/admin/news")]
public class AdminNewsController : AdminControllerBase
{
    private readonly IAppDbContext _db;

    public AdminNewsController(IAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<AdminNewsListItemDto[]>> GetAll(CancellationToken ct)
    {
        var items = await _db.NewsArticles.AsNoTracking()
            .OrderByDescending(a => a.UpdatedAt ?? a.CreatedAt)
            .Select(a => new AdminNewsListItemDto(
                a.Id, a.Slug, a.Title, a.IsPublished, a.PublishedAt,
                a.Category != null ? a.Category.Name : null,
                a.CoverImage != null ? a.CoverImage.Url : null,
                a.UpdatedAt))
            .ToArrayAsync(ct);

        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminNewsDetailDto>> Get(Guid id, CancellationToken ct)
    {
        var article = await QueryDetailAsync(id, ct);
        return article is null ? NotFound() : Ok(article);
    }

    private Task<AdminNewsDetailDto?> QueryDetailAsync(Guid id, CancellationToken ct) =>
        _db.NewsArticles.AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new AdminNewsDetailDto(
                a.Id, a.Slug, a.Title, a.TitleEn, a.Excerpt, a.ExcerptEn, a.Content, a.ContentEn,
                a.ReadingTimeMinutes, a.IsPublished, a.PublishedAt, a.CoverImageId,
                a.CoverImage != null ? a.CoverImage.Url : null,
                a.CategoryId, a.Tags.Select(t => t.Id).ToArray(),
                a.MetaTitle, a.MetaTitleEn, a.MetaDescription, a.MetaDescriptionEn, a.OgImageUrl))
            .FirstOrDefaultAsync(ct)!;

    [HttpPost]
    public async Task<ActionResult<AdminNewsDetailDto>> Create([FromBody] AdminNewsUpsertDto dto, CancellationToken ct)
    {
        if (await _db.NewsArticles.AnyAsync(a => a.Slug == dto.Slug, ct))
            return Conflict($"An article with slug '{dto.Slug}' already exists.");

        var validationError = await ValidateReferencesAsync(dto, ct);
        if (validationError is not null)
            return BadRequest(validationError);

        var article = new NewsArticle();
        Apply(article, dto);
        article.Tags = await LoadTagsAsync(dto.TagIds, ct);
        if (dto.IsPublished)
            article.PublishedAt = DateTimeOffset.UtcNow;

        _db.NewsArticles.Add(article);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(Get), new { id = article.Id }, await ToDetailAsync(article.Id, ct));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AdminNewsDetailDto>> Update(Guid id, [FromBody] AdminNewsUpsertDto dto, CancellationToken ct)
    {
        var article = await _db.NewsArticles.Include(a => a.Tags).FirstOrDefaultAsync(a => a.Id == id, ct);
        if (article is null)
            return NotFound();

        if (await _db.NewsArticles.AnyAsync(a => a.Slug == dto.Slug && a.Id != id, ct))
            return Conflict($"An article with slug '{dto.Slug}' already exists.");

        var validationError = await ValidateReferencesAsync(dto, ct);
        if (validationError is not null)
            return BadRequest(validationError);

        var wasPublished = article.IsPublished;
        Apply(article, dto);
        article.Tags = await LoadTagsAsync(dto.TagIds, ct);
        // First publish stamps the date; later edits keep the original publish date.
        if (dto.IsPublished && !wasPublished && article.PublishedAt is null)
            article.PublishedAt = DateTimeOffset.UtcNow;
        article.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Ok(await ToDetailAsync(article.Id, ct));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var article = await _db.NewsArticles.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (article is null)
            return NotFound();

        _db.NewsArticles.Remove(article);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static void Apply(NewsArticle article, AdminNewsUpsertDto dto)
    {
        article.Title = dto.Title;
        article.TitleEn = dto.TitleEn;
        article.Slug = dto.Slug;
        article.Excerpt = dto.Excerpt;
        article.ExcerptEn = dto.ExcerptEn;
        article.Content = dto.Content;
        article.ContentEn = dto.ContentEn;
        article.ReadingTimeMinutes = dto.ReadingTimeMinutes;
        article.IsPublished = dto.IsPublished;
        article.CoverImageId = dto.CoverImageId;
        article.CategoryId = dto.CategoryId;
        article.MetaTitle = dto.MetaTitle;
        article.MetaTitleEn = dto.MetaTitleEn;
        article.MetaDescription = dto.MetaDescription;
        article.MetaDescriptionEn = dto.MetaDescriptionEn;
        article.OgImageUrl = dto.OgImageUrl;
    }

    private async Task<string?> ValidateReferencesAsync(AdminNewsUpsertDto dto, CancellationToken ct)
    {
        if (!await _db.MediaAssets.AnyAsync(m => m.Id == dto.CoverImageId, ct))
            return "The selected cover image does not exist.";
        if (!await _db.Categories.AnyAsync(c => c.Id == dto.CategoryId, ct))
            return "The selected category does not exist.";
        return null;
    }

    private async Task<List<Tag>> LoadTagsAsync(Guid[] tagIds, CancellationToken ct)
    {
        if (tagIds.Length == 0)
            return new List<Tag>();
        return await _db.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync(ct);
    }

    private async Task<AdminNewsDetailDto> ToDetailAsync(Guid id, CancellationToken ct) =>
        (await QueryDetailAsync(id, ct))!;
}
