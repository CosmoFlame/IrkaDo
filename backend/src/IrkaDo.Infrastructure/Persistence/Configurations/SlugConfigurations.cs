using IrkaDo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IrkaDo.Infrastructure.Persistence.Configurations;

public class NewsArticleConfiguration : IEntityTypeConfiguration<NewsArticle>
{
    public void Configure(EntityTypeBuilder<NewsArticle> builder)
    {
        builder.HasIndex(a => a.Slug).IsUnique();
        builder.HasMany(a => a.Tags).WithMany();
        builder.HasMany(a => a.Links).WithOne()
            .HasForeignKey(l => l.NewsArticleId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class TravelGuideConfiguration : IEntityTypeConfiguration<TravelGuide>
{
    public void Configure(EntityTypeBuilder<TravelGuide> builder)
    {
        builder.HasIndex(g => g.Slug).IsUnique();
        builder.HasMany(g => g.PreviewImages).WithMany();
        builder.HasMany(g => g.Links).WithOne()
            .HasForeignKey(l => l.TravelGuideId).OnDelete(DeleteBehavior.Cascade);
        builder.Property(g => g.PriceAmount).HasPrecision(10, 2);
    }
}

public class CollaborationConfiguration : IEntityTypeConfiguration<Collaboration>
{
    public void Configure(EntityTypeBuilder<Collaboration> builder)
    {
        builder.HasMany(c => c.Links).WithOne()
            .HasForeignKey(l => l.CollaborationId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.HasIndex(p => p.Slug).IsUnique();
    }
}

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.HasIndex(p => p.ProviderSessionId).IsUnique();
        builder.Property(p => p.AmountPaid).HasPrecision(10, 2);
    }
}
