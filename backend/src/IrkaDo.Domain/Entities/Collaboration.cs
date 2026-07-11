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

    public Guid CoverImageId { get; set; }
    public MediaAsset? CoverImage { get; set; }

    public ICollection<ContentLink> Links { get; set; } = new List<ContentLink>();
}
