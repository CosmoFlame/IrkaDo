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
        var items = await _db.SocialLinks.AsNoTracking()
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new SocialLinkDto(s.Platform.ToString(), s.Url, s.Description, s.FollowerCount))
            .ToArrayAsync(cancellationToken);

        return Ok(items);
    }
}
