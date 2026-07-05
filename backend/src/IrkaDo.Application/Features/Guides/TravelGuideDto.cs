namespace IrkaDo.Application.Features.Guides;

public record TravelGuideSummaryDto(
    string Slug,
    string Title,
    string Country,
    string? City,
    string Continent,
    int DurationDays,
    string? Difficulty,
    bool IsPremium,
    decimal? PriceAmount,
    string PriceCurrency,
    string? CoverImageUrl);

public record TravelGuideDetailDto(
    string Slug,
    string Title,
    string Country,
    string? City,
    string Continent,
    string Description,
    string? WhatsIncluded,
    int DurationDays,
    string? Difficulty,
    bool IsPremium,
    decimal? PriceAmount,
    string PriceCurrency,
    string? CoverImageUrl,
    string[] PreviewImageUrls,
    DateTimeOffset? LastUpdatedAt,
    string? MetaTitle,
    string? MetaDescription,
    string? OgImageUrl);
