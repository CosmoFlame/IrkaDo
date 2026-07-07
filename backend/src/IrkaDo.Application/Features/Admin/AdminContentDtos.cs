using IrkaDo.Domain.Entities;

namespace IrkaDo.Application.Features.Admin;

// --- News ---

public record AdminNewsListItemDto(
    Guid Id,
    string Slug,
    string Title,
    bool IsPublished,
    DateTimeOffset? PublishedAt,
    string? Category,
    string? CoverImageUrl,
    DateTimeOffset? UpdatedAt);

public record AdminNewsDetailDto(
    Guid Id,
    string Slug,
    string Title,
    string Excerpt,
    string Content,
    int ReadingTimeMinutes,
    bool IsPublished,
    DateTimeOffset? PublishedAt,
    Guid CoverImageId,
    string? CoverImageUrl,
    Guid CategoryId,
    Guid[] TagIds,
    string? MetaTitle,
    string? MetaDescription,
    string? OgImageUrl);

public record AdminNewsUpsertDto(
    string Title,
    string Slug,
    string Excerpt,
    string Content,
    int ReadingTimeMinutes,
    bool IsPublished,
    Guid CoverImageId,
    Guid CategoryId,
    Guid[] TagIds,
    string? MetaTitle,
    string? MetaDescription,
    string? OgImageUrl);

// --- Guides ---

public record AdminGuideFileDto(Guid Id, string FileName, long SizeBytes);

public record AdminGuideListItemDto(
    Guid Id,
    string Slug,
    string Title,
    string Country,
    string Continent,
    bool IsPremium,
    decimal? PriceAmount,
    string PriceCurrency,
    bool IsPublished,
    bool IsFeatured,
    string? CoverImageUrl,
    int FileCount,
    DateTimeOffset? UpdatedAt);

public record AdminGuideDetailDto(
    Guid Id,
    string Slug,
    string Title,
    string Country,
    string? City,
    string Continent,
    string Description,
    string? WhatsIncluded,
    int DurationDays,
    GuideDifficulty? Difficulty,
    bool IsPremium,
    decimal? PriceAmount,
    string PriceCurrency,
    bool IsPublished,
    bool IsFeatured,
    DateTimeOffset? LastUpdatedAt,
    Guid CoverImageId,
    string? CoverImageUrl,
    Guid[] PreviewImageIds,
    AdminGuideFileDto[] Files,
    string? MetaTitle,
    string? MetaDescription,
    string? OgImageUrl);

public record AdminGuideUpsertDto(
    string Title,
    string Slug,
    string Country,
    string? City,
    string Continent,
    string Description,
    string? WhatsIncluded,
    int DurationDays,
    GuideDifficulty? Difficulty,
    bool IsPremium,
    decimal? PriceAmount,
    string PriceCurrency,
    bool IsPublished,
    bool IsFeatured,
    Guid CoverImageId,
    Guid[] PreviewImageIds,
    string? MetaTitle,
    string? MetaDescription,
    string? OgImageUrl);
