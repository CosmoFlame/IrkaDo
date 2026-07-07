using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers.Admin;

[Route("api/v1/admin/home-sections")]
public class AdminHomeSectionsController : AdminControllerBase
{
    private readonly IAppDbContext _db;

    public AdminHomeSectionsController(IAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<AdminHomeSectionDto[]>> GetAll(CancellationToken ct)
    {
        var items = await _db.HomeSections.AsNoTracking()
            .OrderBy(s => s.Type)
            .Select(s => new AdminHomeSectionDto(
                s.Id, s.Type, s.Headline, s.HeadlineEn, s.Body, s.BodyEn, s.ContentJson, s.ContentJsonEn,
                s.BackgroundMediaId, s.BackgroundMedia != null ? s.BackgroundMedia.Url : null))
            .ToArrayAsync(ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminHomeSectionDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await QueryDtoAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AdminHomeSectionDto>> Update(Guid id, [FromBody] AdminHomeSectionUpdateDto dto, CancellationToken ct)
    {
        var section = await _db.HomeSections.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (section is null)
            return NotFound();

        if (dto.BackgroundMediaId is { } mediaId && !await _db.MediaAssets.AnyAsync(m => m.Id == mediaId, ct))
            return BadRequest("The selected background image does not exist.");

        section.Headline = dto.Headline;
        section.HeadlineEn = dto.HeadlineEn;
        section.Body = dto.Body;
        section.BodyEn = dto.BodyEn;
        section.ContentJson = string.IsNullOrWhiteSpace(dto.ContentJson) ? "{}" : dto.ContentJson;
        section.ContentJsonEn = string.IsNullOrWhiteSpace(dto.ContentJsonEn) ? null : dto.ContentJsonEn;
        section.BackgroundMediaId = dto.BackgroundMediaId;
        section.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Ok((await QueryDtoAsync(id, ct))!);
    }

    private Task<AdminHomeSectionDto?> QueryDtoAsync(Guid id, CancellationToken ct) =>
        _db.HomeSections.AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new AdminHomeSectionDto(
                s.Id, s.Type, s.Headline, s.HeadlineEn, s.Body, s.BodyEn, s.ContentJson, s.ContentJsonEn,
                s.BackgroundMediaId, s.BackgroundMedia != null ? s.BackgroundMedia.Url : null))
            .FirstOrDefaultAsync(ct)!;
}
