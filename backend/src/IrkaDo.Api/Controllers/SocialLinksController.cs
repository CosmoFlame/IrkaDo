using IrkaDo.Api.Localization;
using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers;

[ApiController]
[Route("api/v1/social-links")]
public class SocialLinksController : ControllerBase
{
    private readonly IAppDbContext _db;

    public SocialLinksController(IAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<SocialLinkDto[]>> GetAll(CancellationToken cancellationToken = default)
    {
        var en = Request.IsEnglish();
        var items = await _db.SocialLinks.AsNoTracking()
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new SocialLinkDto(
                s.Platform.ToString(), s.Url,
                en && s.DescriptionEn != null ? s.DescriptionEn : s.Description,
                s.FollowerCount))
            .ToArrayAsync(cancellationToken);

        return Ok(items);
    }
}
