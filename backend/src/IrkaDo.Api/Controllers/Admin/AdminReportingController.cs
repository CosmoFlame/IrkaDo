using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Application.Features;
using IrkaDo.Application.Features.Admin;
using IrkaDo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Api.Controllers.Admin;

[Route("api/v1/admin/purchases")]
public class AdminPurchasesController : AdminControllerBase
{
    private readonly IAppDbContext _db;

    public AdminPurchasesController(IAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<PagedResult<AdminPurchaseDto>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default)
    {
        var query = _db.Purchases.AsNoTracking();
        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new AdminPurchaseDto(
                p.Id, p.TravelGuide != null ? p.TravelGuide.Title : null,
                p.CustomerEmail, p.AmountPaid, p.Currency, p.Status,
                p.PaymentProvider, p.ProviderSessionId, p.DeliveredAt, p.CreatedAt))
            .ToArrayAsync(ct);

        return Ok(new PagedResult<AdminPurchaseDto>(items, page, pageSize, totalCount));
    }
}

[Route("api/v1/admin/download-logs")]
public class AdminDownloadLogsController : AdminControllerBase
{
    private readonly IAppDbContext _db;

    public AdminDownloadLogsController(IAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<PagedResult<AdminDownloadLogDto>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default)
    {
        var query = _db.DownloadLogs.AsNoTracking();
        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new AdminDownloadLogDto(
                d.Id, d.TravelGuide != null ? d.TravelGuide.Title : null,
                d.Email, d.IpAddress, d.CreatedAt))
            .ToArrayAsync(ct);

        return Ok(new PagedResult<AdminDownloadLogDto>(items, page, pageSize, totalCount));
    }
}

[Route("api/v1/admin/dashboard")]
public class AdminDashboardController : AdminControllerBase
{
    private readonly IAppDbContext _db;

    public AdminDashboardController(IAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<AdminDashboardDto>> Get(CancellationToken ct)
    {
        var dto = new AdminDashboardDto(
            NewsTotal: await _db.NewsArticles.CountAsync(ct),
            NewsPublished: await _db.NewsArticles.CountAsync(a => a.IsPublished, ct),
            GuidesTotal: await _db.TravelGuides.CountAsync(ct),
            GuidesPublished: await _db.TravelGuides.CountAsync(g => g.IsPublished, ct),
            PremiumGuides: await _db.TravelGuides.CountAsync(g => g.IsPremium, ct),
            Collaborations: await _db.Collaborations.CountAsync(ct),
            SocialLinks: await _db.SocialLinks.CountAsync(ct),
            MediaAssets: await _db.MediaAssets.CountAsync(ct),
            PurchasesCompleted: await _db.Purchases.CountAsync(p => p.Status == PurchaseStatus.Completed, ct),
            Revenue: await _db.Purchases.Where(p => p.Status == PurchaseStatus.Completed).SumAsync(p => (decimal?)p.AmountPaid, ct) ?? 0m,
            DownloadsTotal: await _db.DownloadLogs.CountAsync(ct));

        return Ok(dto);
    }
}
