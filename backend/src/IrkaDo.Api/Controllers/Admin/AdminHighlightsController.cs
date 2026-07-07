using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features.Admin;
using IrkaDo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers.Admin;

[Route("api/v1/admin/highlights")]
public class AdminHighlightsController : AdminControllerBase
{
    private readonly IAppDbContext _db;

    public AdminHighlightsController(IAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<AdminHighlightDto[]>> GetAll(CancellationToken ct)
    {
        var items = await _db.TravelHighlights.AsNoTracking()
            .OrderBy(h => h.DisplayOrder)
            .Select(h => new AdminHighlightDto(
                h.Id, h.Destination, h.Caption, h.DisplayOrder, h.IsPublished,
                h.ImageId, h.Image != null ? h.Image.Url : null))
            .ToArrayAsync(ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminHighlightDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await _db.TravelHighlights.AsNoTracking()
            .Where(h => h.Id == id)
            .Select(h => new AdminHighlightDto(
                h.Id, h.Destination, h.Caption, h.DisplayOrder, h.IsPublished,
                h.ImageId, h.Image != null ? h.Image.Url : null))
            .FirstOrDefaultAsync(ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<AdminHighlightDto>> Create([FromBody] AdminHighlightUpsertDto dto, CancellationToken ct)
    {
        if (!await _db.MediaAssets.AnyAsync(m => m.Id == dto.ImageId, ct))
            return BadRequest("The selected image does not exist.");

        var highlight = new TravelHighlight();
        Apply(highlight, dto);
        _db.TravelHighlights.Add(highlight);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = highlight.Id }, await GetDtoAsync(highlight.Id, ct));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AdminHighlightDto>> Update(Guid id, [FromBody] AdminHighlightUpsertDto dto, CancellationToken ct)
    {
        var highlight = await _db.TravelHighlights.FirstOrDefaultAsync(h => h.Id == id, ct);
        if (highlight is null)
            return NotFound();
        if (!await _db.MediaAssets.AnyAsync(m => m.Id == dto.ImageId, ct))
            return BadRequest("The selected image does not exist.");

        Apply(highlight, dto);
        highlight.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Ok(await GetDtoAsync(highlight.Id, ct));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var highlight = await _db.TravelHighlights.FirstOrDefaultAsync(h => h.Id == id, ct);
        if (highlight is null)
            return NotFound();
        _db.TravelHighlights.Remove(highlight);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static void Apply(TravelHighlight highlight, AdminHighlightUpsertDto dto)
    {
        highlight.Destination = dto.Destination;
        highlight.Caption = dto.Caption;
        highlight.DisplayOrder = dto.DisplayOrder;
        highlight.IsPublished = dto.IsPublished;
        highlight.ImageId = dto.ImageId;
    }

    private async Task<AdminHighlightDto> GetDtoAsync(Guid id, CancellationToken ct) =>
        (await _db.TravelHighlights.AsNoTracking()
            .Where(h => h.Id == id)
            .Select(h => new AdminHighlightDto(
                h.Id, h.Destination, h.Caption, h.DisplayOrder, h.IsPublished,
                h.ImageId, h.Image != null ? h.Image.Url : null))
            .FirstOrDefaultAsync(ct))!;
}
