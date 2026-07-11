using IrkaDo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<NewsArticle> NewsArticles { get; }
    DbSet<Category> Categories { get; }
    DbSet<Tag> Tags { get; }
    DbSet<TravelGuide> TravelGuides { get; }
    DbSet<GuideFile> GuideFiles { get; }
    DbSet<Collaboration> Collaborations { get; }
    DbSet<SocialLink> SocialLinks { get; }
    DbSet<Purchase> Purchases { get; }
    DbSet<DownloadLog> DownloadLogs { get; }
    DbSet<MediaAsset> MediaAssets { get; }
    DbSet<Page> Pages { get; }
    DbSet<HomeSection> HomeSections { get; }
    DbSet<ContentLink> ContentLinks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
