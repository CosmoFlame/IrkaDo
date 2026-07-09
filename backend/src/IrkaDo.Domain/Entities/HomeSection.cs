using IrkaDo.Domain.Common;

namespace IrkaDo.Domain.Entities;

public enum HomeSectionType
{
    Hero,
    About,
    Contact
}

/// <summary>
/// Free-form content blocks for singleton home sections (Hero/About/Contact) whose
/// fields differ enough that a generic JSON payload is simpler than one table per section.
/// </summary>
public class HomeSection : BaseEntity
{
    public HomeSectionType Type { get; set; }
    public string Headline { get; set; } = string.Empty;
    public string? HeadlineEn { get; set; }
    public string Body { get; set; } = string.Empty;
    public string? BodyEn { get; set; }
    public string ContentJson { get; set; } = "{}";
    public string? ContentJsonEn { get; set; }

    /// <summary>Business/contact email surfaced on the Contact section. Not translatable.</summary>
    public string? ContactEmail { get; set; }

    public Guid? BackgroundMediaId { get; set; }
    public MediaAsset? BackgroundMedia { get; set; }
}
