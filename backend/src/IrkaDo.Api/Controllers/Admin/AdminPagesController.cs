using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features.Admin;
using IrkaDo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers.Admin;

[Route("api/v1/admin/pages")]
public class AdminPagesController : AdminControllerBase
{
    private readonly IAppDbContext _db;

    public AdminPagesController(IAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<AdminPageDto[]>> GetAll(CancellationToken ct)
    {
        var items = await _db.Pages.AsNoTracking()
            .OrderBy(p => p.Slug)
            .Select(p => new AdminPageDto(p.Id, p.Slug, p.Title, p.TitleEn, p.MetaTitle, p.MetaTitleEn, p.MetaDescription, p.MetaDescriptionEn, p.OgImageUrl))
            .ToArrayAsync(ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminPageDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await _db.Pages.AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new AdminPageDto(p.Id, p.Slug, p.Title, p.TitleEn, p.MetaTitle, p.MetaTitleEn, p.MetaDescription, p.MetaDescriptionEn, p.OgImageUrl))
            .FirstOrDefaultAsync(ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<AdminPageDto>> Create([FromBody] AdminPageUpsertDto dto, CancellationToken ct)
    {
        if (await _db.Pages.AnyAsync(p => p.Slug == dto.Slug, ct))
            return Conflict($"A page with slug '{dto.Slug}' already exists.");

        var page = new Page();
        Apply(page, dto);
        _db.Pages.Add(page);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = page.Id },
            new AdminPageDto(page.Id, page.Slug, page.Title, page.TitleEn, page.MetaTitle, page.MetaTitleEn, page.MetaDescription, page.MetaDescriptionEn, page.OgImageUrl));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AdminPageDto>> Update(Guid id, [FromBody] AdminPageUpsertDto dto, CancellationToken ct)
    {
        var page = await _db.Pages.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (page is null)
            return NotFound();
        if (await _db.Pages.AnyAsync(p => p.Slug == dto.Slug && p.Id != id, ct))
            return Conflict($"A page with slug '{dto.Slug}' already exists.");

        Apply(page, dto);
        page.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Ok(new AdminPageDto(page.Id, page.Slug, page.Title, page.TitleEn, page.MetaTitle, page.MetaTitleEn, page.MetaDescription, page.MetaDescriptionEn, page.OgImageUrl));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var page = await _db.Pages.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (page is null)
            return NotFound();
        _db.Pages.Remove(page);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static void Apply(Page page, AdminPageUpsertDto dto)
    {
        page.Slug = dto.Slug;
        page.Title = dto.Title;
        page.TitleEn = dto.TitleEn;
        page.MetaTitle = dto.MetaTitle;
        page.MetaTitleEn = dto.MetaTitleEn;
        page.MetaDescription = dto.MetaDescription;
        page.MetaDescriptionEn = dto.MetaDescriptionEn;
        page.OgImageUrl = dto.OgImageUrl;
    }
}
