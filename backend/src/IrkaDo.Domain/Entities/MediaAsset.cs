using IrkaDo.Domain.Common;

namespace IrkaDo.Domain.Entities;

public enum MediaAssetType
{
    Image,
    Video,
    Document
}

public class MediaAsset : BaseEntity
{
    public string Url { get; set; } = string.Empty;
    public MediaAssetType Type { get; set; } = MediaAssetType.Image;
    public string? AltText { get; set; }
    public string? AltTextEn { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
}
