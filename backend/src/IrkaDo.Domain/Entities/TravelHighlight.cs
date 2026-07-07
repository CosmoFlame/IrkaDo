using IrkaDo.Domain.Common;

namespace IrkaDo.Domain.Entities;

public class TravelHighlight : BaseEntity
{
    public string Destination { get; set; } = string.Empty;
    public string? DestinationEn { get; set; }
    public string Caption { get; set; } = string.Empty;
    public string? CaptionEn { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }

    public Guid ImageId { get; set; }
    public MediaAsset? Image { get; set; }
}
