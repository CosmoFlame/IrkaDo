using IrkaDo.Domain.Entities;

namespace IrkaDo.Application.Features.Admin;

// --- Collaborations ---

public record AdminCollaborationDto(
    Guid Id,
    string BrandName,
    string Description,
    string? DescriptionEn,
    string? Testimonial,
    string? TestimonialEn,
    int DisplayOrder,
    bool IsPublished,
    Guid CoverImageId,
    string? CoverImageUrl,
    AdminLinkDto[] Links);

public record AdminCollaborationUpsertDto(
    string BrandName,
    string Description,
    string? DescriptionEn,
    string? Testimonial,
    string? TestimonialEn,
    int DisplayOrder,
    bool IsPublished,
    Guid CoverImageId,
    AdminLinkDto[] Links);

// --- Social links ---

public record AdminSocialLinkDto(
    Guid Id,
    SocialPlatform Platform,
    string Url,
    string? Description,
    string? DescriptionEn,
    int? FollowerCount,
    int DisplayOrder);

public record AdminSocialLinkUpsertDto(
    SocialPlatform Platform,
    string Url,
    string? Description,
    string? DescriptionEn,
    int? FollowerCount,
    int DisplayOrder);

// --- Home sections (singletons: Hero / About / Contact) ---

public record AdminHomeSectionDto(
    Guid Id,
    HomeSectionType Type,
    string Headline,
    string? HeadlineEn,
    string Body,
    string? BodyEn,
    string ContentJson,
    string? ContentJsonEn,
    string? ContactEmail,
    Guid? BackgroundMediaId,
    string? BackgroundMediaUrl);

public record AdminHomeSectionUpdateDto(
    string Headline,
    string? HeadlineEn,
    string Body,
    string? BodyEn,
    string ContentJson,
    string? ContentJsonEn,
    string? ContactEmail,
    Guid? BackgroundMediaId);
