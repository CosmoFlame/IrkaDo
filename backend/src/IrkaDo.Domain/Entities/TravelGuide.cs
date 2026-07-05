using IrkaDo.Domain.Common;

namespace IrkaDo.Domain.Entities;

public enum GuideDifficulty
{
    Easy,
    Moderate,
    Challenging
}

public class TravelGuide : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? City { get; set; }
    public string Continent { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? WhatsIncluded { get; set; }
    public int DurationDays { get; set; }
    public GuideDifficulty? Difficulty { get; set; }

    public bool IsPremium { get; set; }
    public decimal? PriceAmount { get; set; }
    public string PriceCurrency { get; set; } = "USD";

    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }
    public DateTimeOffset? LastUpdatedAt { get; set; }

    public Guid CoverImageId { get; set; }
    public MediaAsset? CoverImage { get; set; }

    public ICollection<MediaAsset> PreviewImages { get; set; } = new List<MediaAsset>();
    public ICollection<GuideFile> Files { get; set; } = new List<GuideFile>();

    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? OgImageUrl { get; set; }
}
