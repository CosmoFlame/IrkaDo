using System.Linq.Expressions;
using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features.Admin;
using IrkaDo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers.Admin;

[Route("api/v1/admin/collaborations")]
public class AdminCollaborationsController : AdminControllerBase
{
    private readonly IAppDbContext _db;

    public AdminCollaborationsController(IAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<AdminCollaborationDto[]>> GetAll(CancellationToken ct)
    {
        var items = await _db.Collaborations.AsNoTracking()
            .OrderBy(c => c.DisplayOrder)
            .Select(Projection)
            .ToArrayAsync(ct);

        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminCollaborationDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await _db.Collaborations.AsNoTracking()
            .Where(c => c.Id == id).Select(Projection).FirstOrDefaultAsync(ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<AdminCollaborationDto>> Create([FromBody] AdminCollaborationUpsertDto dto, CancellationToken ct)
    {
        if (!await _db.MediaAssets.AnyAsync(m => m.Id == dto.CoverImageId, ct))
            return BadRequest("The selected cover image does not exist.");

        var collab = new Collaboration();
        Apply(collab, dto);
        collab.Links = ContentLinkMapping.ToEntities(dto.Links);

        _db.Collaborations.Add(collab);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = collab.Id }, await GetDtoAsync(collab.Id, ct));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AdminCollaborationDto>> Update(Guid id, [FromBody] AdminCollaborationUpsertDto dto, CancellationToken ct)
    {
        var collab = await _db.Collaborations.Include(c => c.Links).FirstOrDefaultAsync(c => c.Id == id, ct);
        if (collab is null)
            return NotFound();
        if (!await _db.MediaAssets.AnyAsync(m => m.Id == dto.CoverImageId, ct))
            return BadRequest("The selected cover image does not exist.");

        Apply(collab, dto);
        ContentLinkMapping.ReplaceLinks(_db, collab.Links, dto.Links, l => l.CollaborationId = collab.Id);
        collab.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Ok(await GetDtoAsync(collab.Id, ct));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var collab = await _db.Collaborations.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (collab is null)
            return NotFound();
        _db.Collaborations.Remove(collab);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static void Apply(Collaboration collab, AdminCollaborationUpsertDto dto)
    {
        collab.BrandName = dto.BrandName;
        collab.Description = dto.Description;
        collab.DescriptionEn = dto.DescriptionEn;
        collab.Testimonial = dto.Testimonial;
        collab.TestimonialEn = dto.TestimonialEn;
        collab.DisplayOrder = dto.DisplayOrder;
        collab.IsPublished = dto.IsPublished;
        collab.CoverImageId = dto.CoverImageId;
    }

    private async Task<AdminCollaborationDto> GetDtoAsync(Guid id, CancellationToken ct) =>
        (await _db.Collaborations.AsNoTracking().Where(c => c.Id == id).Select(Projection).FirstOrDefaultAsync(ct))!;

    private static readonly Expression<Func<Collaboration, AdminCollaborationDto>> Projection = c => new AdminCollaborationDto(
        c.Id, c.BrandName, c.Description, c.DescriptionEn, c.Testimonial, c.TestimonialEn,
        c.DisplayOrder, c.IsPublished,
        c.CoverImageId, c.CoverImage != null ? c.CoverImage.Url : null,
        c.Links.OrderBy(l => l.DisplayOrder)
            .Select(l => new AdminLinkDto(l.Url, l.Title, l.TitleEn, l.DisplayOrder)).ToArray());
}
