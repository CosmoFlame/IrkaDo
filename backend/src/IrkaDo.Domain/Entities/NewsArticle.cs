using IrkaDo.Domain.Common;

namespace IrkaDo.Domain.Entities;

public class NewsArticle : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int ReadingTimeMinutes { get; set; }
    public bool IsPublished { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }

    public Guid CoverImageId { get; set; }
    public MediaAsset? CoverImage { get; set; }

    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? OgImageUrl { get; set; }
}
