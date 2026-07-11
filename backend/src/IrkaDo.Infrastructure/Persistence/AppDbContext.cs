using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IrkaDo.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<NewsArticle> NewsArticles => Set<NewsArticle>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<TravelGuide> TravelGuides => Set<TravelGuide>();
    public DbSet<GuideFile> GuideFiles => Set<GuideFile>();
    public DbSet<Collaboration> Collaborations => Set<Collaboration>();
    public DbSet<SocialLink> SocialLinks => Set<SocialLink>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<DownloadLog> DownloadLogs => Set<DownloadLog>();
    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<HomeSection> HomeSections => Set<HomeSection>();
    public DbSet<ContentLink> ContentLinks => Set<ContentLink>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
