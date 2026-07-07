using IrkaDo.Domain.Common;

namespace IrkaDo.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string Slug { get; set; } = string.Empty;
}
