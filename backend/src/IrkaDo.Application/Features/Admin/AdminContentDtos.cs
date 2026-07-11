using IrkaDo.Domain.Entities;

namespace IrkaDo.Application.Features.Admin;

// --- Content links (shared by news, guides, collaborations) ---
// Managed inline with the parent: the full set is sent on every upsert and replaced wholesale.

public record AdminLinkDto(string Url, string? Title, string? TitleEn, int DisplayOrder);

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
    string? TitleEn,
    string Excerpt,
    string? ExcerptEn,
    string Content,
    string? ContentEn,
    int ReadingTimeMinutes,
    bool IsPublished,
    DateTimeOffset? PublishedAt,
    Guid CoverImageId,
    string? CoverImageUrl,
    Guid CategoryId,
    Guid[] TagIds,
    AdminLinkDto[] Links,
    string? MetaTitle,
    string? MetaTitleEn,
    string? MetaDescription,
    string? MetaDescriptionEn,
    string? OgImageUrl);

public record AdminNewsUpsertDto(
    string Title,
    string? TitleEn,
    string Slug,
    string Excerpt,
    string? ExcerptEn,
    string Content,
    string? ContentEn,
    int ReadingTimeMinutes,
    bool IsPublished,
    Guid CoverImageId,
    Guid CategoryId,
    Guid[] TagIds,
    AdminLinkDto[] Links,
    string? MetaTitle,
    string? MetaTitleEn,
    string? MetaDescription,
    string? MetaDescriptionEn,
    string? OgImageUrl);

// --- Guides ---

public record AdminGuideFileDto(Guid Id, string FileName, long SizeBytes);

public record AdminGuideListItemDto(
    Guid Id,
    string Slug,
    string Title,
    string Country,
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
    string? TitleEn,
    string Country,
    string? CountryEn,
    string? City,
    string? CityEn,
    string Description,
    string? DescriptionEn,
    string? WhatsIncluded,
    string? WhatsIncludedEn,
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
    AdminLinkDto[] Links,
    string? MetaTitle,
    string? MetaTitleEn,
    string? MetaDescription,
    string? MetaDescriptionEn,
    string? OgImageUrl);

public record AdminGuideUpsertDto(
    string Title,
    string? TitleEn,
    string Slug,
    string Country,
    string? CountryEn,
    string? City,
    string? CityEn,
    string Description,
    string? DescriptionEn,
    string? WhatsIncluded,
    string? WhatsIncludedEn,
    int DurationDays,
    GuideDifficulty? Difficulty,
    bool IsPremium,
    decimal? PriceAmount,
    string PriceCurrency,
    bool IsPublished,
    bool IsFeatured,
    Guid CoverImageId,
    Guid[] PreviewImageIds,
    AdminLinkDto[] Links,
    string? MetaTitle,
    string? MetaTitleEn,
    string? MetaDescription,
    string? MetaDescriptionEn,
    string? OgImageUrl);
