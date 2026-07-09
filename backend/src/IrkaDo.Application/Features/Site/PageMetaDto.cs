namespace IrkaDo.Application.Features.Site;

/// <summary>
/// Public, per-page SEO metadata resolved from the <c>Page</c> CMS entity (localized, with fallback
/// to the base language). Lets the frontend drive each page's title/description/OG image from the
/// admin instead of hardcoded copy.
/// </summary>
public record PageMetaDto(
    string Slug,
    string Title,
    string? MetaTitle,
    string? MetaDescription,
    string? OgImageUrl);
