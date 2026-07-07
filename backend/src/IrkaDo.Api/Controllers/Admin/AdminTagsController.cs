using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features.Admin;
using IrkaDo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers.Admin;

[Route("api/v1/admin/tags")]
public class AdminTagsController : AdminControllerBase
{
    private readonly IAppDbContext _db;

    public AdminTagsController(IAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<AdminTagDto[]>> GetAll(CancellationToken ct)
    {
        var items = await _db.Tags.AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new AdminTagDto(t.Id, t.Name, t.Slug))
            .ToArrayAsync(ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminTagDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await _db.Tags.AsNoTracking()
            .Where(t => t.Id == id).Select(t => new AdminTagDto(t.Id, t.Name, t.Slug)).FirstOrDefaultAsync(ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<AdminTagDto>> Create([FromBody] AdminTagUpsertDto dto, CancellationToken ct)
    {
        if (await _db.Tags.AnyAsync(t => t.Slug == dto.Slug, ct))
            return Conflict($"A tag with slug '{dto.Slug}' already exists.");

        var tag = new Tag { Name = dto.Name, Slug = dto.Slug };
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = tag.Id }, new AdminTagDto(tag.Id, tag.Name, tag.Slug));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AdminTagDto>> Update(Guid id, [FromBody] AdminTagUpsertDto dto, CancellationToken ct)
    {
        var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (tag is null)
            return NotFound();
        if (await _db.Tags.AnyAsync(t => t.Slug == dto.Slug && t.Id != id, ct))
            return Conflict($"A tag with slug '{dto.Slug}' already exists.");

        tag.Name = dto.Name;
        tag.Slug = dto.Slug;
        tag.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Ok(new AdminTagDto(tag.Id, tag.Name, tag.Slug));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (tag is null)
            return NotFound();
        _db.Tags.Remove(tag);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
