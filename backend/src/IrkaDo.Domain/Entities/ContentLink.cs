using IrkaDo.Domain.Common;

namespace IrkaDo.Domain.Entities;

/// <summary>
/// An editor-authored outbound link (e.g. to an Instagram/Telegram post) attached to a
/// news article, travel guide, or collaboration. Each owner can have several. The optional
/// title is the link's display text and is bilingual like the rest of the content.
/// Exactly one of the parent foreign keys is set per row.
/// </summary>
public class ContentLink : BaseEntity
{
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? TitleEn { get; set; }
    public int DisplayOrder { get; set; }

    public Guid? NewsArticleId { get; set; }
    public Guid? TravelGuideId { get; set; }
    public Guid? CollaborationId { get; set; }
}
