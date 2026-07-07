using IrkaDo.Domain.Common;

namespace IrkaDo.Domain.Entities;

public enum SocialPlatform
{
    Instagram,
    TikTok,
    YouTube,
    Telegram,
    Threads
}

public class SocialLink : BaseEntity
{
    public SocialPlatform Platform { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionEn { get; set; }
    public int? FollowerCount { get; set; }
    public int DisplayOrder { get; set; }
}
