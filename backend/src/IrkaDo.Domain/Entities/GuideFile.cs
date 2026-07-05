using IrkaDo.Domain.Common;

namespace IrkaDo.Domain.Entities;

public class GuideFile : BaseEntity
{
    public Guid TravelGuideId { get; set; }
    public TravelGuide? TravelGuide { get; set; }

    public string StorageKey { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
}
