namespace IrkaDo.Application.Features.Guides;

public record TravelGuideSummaryDto(
    string Slug,
    string Title,
    string Country,
    string? City,
    int DurationDays,
    string? Difficulty,
    bool IsPremium,
    decimal? PriceAmount,
    string PriceCurrency,
    string? CoverImageUrl,
    string? CoverImageAlt);

public record TravelGuideDetailDto(
    string Slug,
    string Title,
    string Country,
    string? City,
    string Description,
    string? WhatsIncluded,
    int DurationDays,
    string? Difficulty,
    bool IsPremium,
    decimal? PriceAmount,
    string PriceCurrency,
    string? CoverImageUrl,
    string? CoverImageAlt,
    ImageDto[] PreviewImages,
    LinkDto[] Links,
    DateTimeOffset? LastUpdatedAt,
    string? MetaTitle,
    string? MetaDescription,
    string? OgImageUrl);
