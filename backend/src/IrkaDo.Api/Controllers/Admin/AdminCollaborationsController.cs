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
        if (!await _db.MediaAssets.AnyAsync(m => m.Id == dto.LogoId, ct))
            return BadRequest("The selected logo image does not exist.");

        var collab = new Collaboration();
        Apply(collab, dto);
        collab.CampaignImages = await LoadImagesAsync(dto.CampaignImageIds, ct);

        _db.Collaborations.Add(collab);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = collab.Id }, await GetDtoAsync(collab.Id, ct));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AdminCollaborationDto>> Update(Guid id, [FromBody] AdminCollaborationUpsertDto dto, CancellationToken ct)
    {
        var collab = await _db.Collaborations.Include(c => c.CampaignImages).FirstOrDefaultAsync(c => c.Id == id, ct);
        if (collab is null)
            return NotFound();
        if (!await _db.MediaAssets.AnyAsync(m => m.Id == dto.LogoId, ct))
            return BadRequest("The selected logo image does not exist.");

        Apply(collab, dto);
        collab.CampaignImages = await LoadImagesAsync(dto.CampaignImageIds, ct);
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
        collab.Testimonial = dto.Testimonial;
        collab.DisplayOrder = dto.DisplayOrder;
        collab.IsPublished = dto.IsPublished;
        collab.LogoId = dto.LogoId;
    }

    private async Task<List<MediaAsset>> LoadImagesAsync(Guid[] ids, CancellationToken ct) =>
        ids.Length == 0 ? new List<MediaAsset>() : await _db.MediaAssets.Where(m => ids.Contains(m.Id)).ToListAsync(ct);

    private async Task<AdminCollaborationDto> GetDtoAsync(Guid id, CancellationToken ct) =>
        (await _db.Collaborations.AsNoTracking().Where(c => c.Id == id).Select(Projection).FirstOrDefaultAsync(ct))!;

    private static readonly Expression<Func<Collaboration, AdminCollaborationDto>> Projection = c => new AdminCollaborationDto(
        c.Id, c.BrandName, c.Description, c.Testimonial, c.DisplayOrder, c.IsPublished,
        c.LogoId, c.Logo != null ? c.Logo.Url : null,
        c.CampaignImages.Select(m => m.Id).ToArray(),
        c.CampaignImages.Select(m => m.Url).ToArray());
}
