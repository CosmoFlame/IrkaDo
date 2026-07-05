using IrkaDo.Domain.Common;

namespace IrkaDo.Domain.Entities;

public class DownloadLog : BaseEntity
{
    public Guid TravelGuideId { get; set; }
    public TravelGuide? TravelGuide { get; set; }

    public string? Email { get; set; }
    public string IpAddress { get; set; } = string.Empty;
}
