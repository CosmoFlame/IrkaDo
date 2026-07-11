using IrkaDo.Api.Localization;
using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features;
using IrkaDo.Application.Features.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers;

[ApiController]
[Route("api/v1/collaborations")]
public class CollaborationsController : ControllerBase
{
    private readonly IAppDbContext _db;

    public CollaborationsController(IAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<CollaborationDto[]>> GetAll(CancellationToken cancellationToken = default)
    {
        var en = Request.IsEnglish();
        var items = await _db.Collaborations.AsNoTracking()
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

        return Ok(items);
    }
}
