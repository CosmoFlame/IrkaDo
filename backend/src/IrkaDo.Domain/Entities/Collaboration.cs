using IrkaDo.Domain.Common;

namespace IrkaDo.Domain.Entities;

public class Collaboration : BaseEntity
{
    public string BrandName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? Testimonial { get; set; }
    public string? TestimonialEn { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }

    public Guid LogoId { get; set; }
    public MediaAsset? Logo { get; set; }

    public ICollection<MediaAsset> CampaignImages { get; set; } = new List<MediaAsset>();
}
