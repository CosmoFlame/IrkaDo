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
                c.Logo != null ? c.Logo.Url : null,
                c.Logo != null ? (en && c.Logo.AltTextEn != null ? c.Logo.AltTextEn : c.Logo.AltText) : null,
                c.CampaignImages
                    .Select(m => new ImageDto(m.Url, en && m.AltTextEn != null ? m.AltTextEn : m.AltText))
                    .ToArray()))
            .ToArrayAsync(cancellationToken);

        return Ok(items);
    }
}
