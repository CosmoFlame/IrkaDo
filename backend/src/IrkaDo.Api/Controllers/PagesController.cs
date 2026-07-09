using IrkaDo.Api.Localization;
using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features.Site;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers;

[ApiController]
[Route("api/v1/pages")]
public class PagesController : ControllerBase
{
    private readonly IAppDbContext _db;

    public PagesController(IAppDbContext db) => _db = db;

    /// <summary>Resolves editable SEO metadata for a page by its slug (e.g. "home", "news", "guides").</summary>
    [HttpGet("{slug}")]
    public async Task<ActionResult<PageMetaDto>> GetBySlug(
        string slug, CancellationToken cancellationToken = default)
    {
        var en = Request.IsEnglish();
        var page = await _db.Pages.AsNoTracking()
            .Where(p => p.Slug == slug)
            .Select(p => new PageMetaDto(
                p.Slug,
                en && p.TitleEn != null ? p.TitleEn : p.Title,
                en && p.MetaTitleEn != null ? p.MetaTitleEn : p.MetaTitle,
                en && p.MetaDescriptionEn != null ? p.MetaDescriptionEn : p.MetaDescription,
                p.OgImageUrl))
            .FirstOrDefaultAsync(cancellationToken);

        return page is null ? NotFound() : Ok(page);
    }
}
