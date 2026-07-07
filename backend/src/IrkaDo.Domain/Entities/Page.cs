using IrkaDo.Domain.Common;

namespace IrkaDo.Domain.Entities;

public class Page : BaseEntity
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? TitleEn { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaTitleEn { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaDescriptionEn { get; set; }
    public string? OgImageUrl { get; set; }
}
