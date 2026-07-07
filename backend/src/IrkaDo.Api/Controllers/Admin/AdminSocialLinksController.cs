using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features.Admin;
using IrkaDo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers.Admin;

[Route("api/v1/admin/social-links")]
public class AdminSocialLinksController : AdminControllerBase
{
    private readonly IAppDbContext _db;

    public AdminSocialLinksController(IAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<AdminSocialLinkDto[]>> GetAll(CancellationToken ct)
    {
        var items = await _db.SocialLinks.AsNoTracking()
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new AdminSocialLinkDto(s.Id, s.Platform, s.Url, s.Description, s.FollowerCount, s.DisplayOrder))
            .ToArrayAsync(ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminSocialLinkDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await _db.SocialLinks.AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new AdminSocialLinkDto(s.Id, s.Platform, s.Url, s.Description, s.FollowerCount, s.DisplayOrder))
            .FirstOrDefaultAsync(ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<AdminSocialLinkDto>> Create([FromBody] AdminSocialLinkUpsertDto dto, CancellationToken ct)
    {
        var link = new SocialLink();
        Apply(link, dto);
        _db.SocialLinks.Add(link);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = link.Id },
            new AdminSocialLinkDto(link.Id, link.Platform, link.Url, link.Description, link.FollowerCount, link.DisplayOrder));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AdminSocialLinkDto>> Update(Guid id, [FromBody] AdminSocialLinkUpsertDto dto, CancellationToken ct)
    {
        var link = await _db.SocialLinks.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (link is null)
            return NotFound();
        Apply(link, dto);
        link.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Ok(new AdminSocialLinkDto(link.Id, link.Platform, link.Url, link.Description, link.FollowerCount, link.DisplayOrder));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var link = await _db.SocialLinks.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (link is null)
            return NotFound();
        _db.SocialLinks.Remove(link);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static void Apply(SocialLink link, AdminSocialLinkUpsertDto dto)
    {
        link.Platform = dto.Platform;
        link.Url = dto.Url;
        link.Description = dto.Description;
        link.FollowerCount = dto.FollowerCount;
        link.DisplayOrder = dto.DisplayOrder;
    }
}
