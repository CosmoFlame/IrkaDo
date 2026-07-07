using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features.Admin;
using IrkaDo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers.Admin;

[Route("api/v1/admin/categories")]
public class AdminCategoriesController : AdminControllerBase
{
    private readonly IAppDbContext _db;

    public AdminCategoriesController(IAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<AdminCategoryDto[]>> GetAll(CancellationToken ct)
    {
        var items = await _db.Categories.AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new AdminCategoryDto(c.Id, c.Name, c.NameEn, c.Slug))
            .ToArrayAsync(ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminCategoryDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await _db.Categories.AsNoTracking()
            .Where(c => c.Id == id).Select(c => new AdminCategoryDto(c.Id, c.Name, c.NameEn, c.Slug)).FirstOrDefaultAsync(ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<AdminCategoryDto>> Create([FromBody] AdminCategoryUpsertDto dto, CancellationToken ct)
    {
        if (await _db.Categories.AnyAsync(c => c.Slug == dto.Slug, ct))
            return Conflict($"A category with slug '{dto.Slug}' already exists.");

        var category = new Category { Name = dto.Name, NameEn = dto.NameEn, Slug = dto.Slug };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = category.Id }, new AdminCategoryDto(category.Id, category.Name, category.NameEn, category.Slug));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AdminCategoryDto>> Update(Guid id, [FromBody] AdminCategoryUpsertDto dto, CancellationToken ct)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category is null)
            return NotFound();
        if (await _db.Categories.AnyAsync(c => c.Slug == dto.Slug && c.Id != id, ct))
            return Conflict($"A category with slug '{dto.Slug}' already exists.");

        category.Name = dto.Name;
        category.NameEn = dto.NameEn;
        category.Slug = dto.Slug;
        category.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Ok(new AdminCategoryDto(category.Id, category.Name, category.NameEn, category.Slug));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category is null)
            return NotFound();

        // Deleting a category cascades to its articles, so refuse while any still reference it.
        if (await _db.NewsArticles.AnyAsync(a => a.CategoryId == id, ct))
            return Conflict("This category is still used by one or more articles. Reassign them first.");

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
