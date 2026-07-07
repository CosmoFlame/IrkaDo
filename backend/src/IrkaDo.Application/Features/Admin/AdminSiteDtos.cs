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
    Guid LogoId,
    string? LogoUrl,
    Guid[] CampaignImageIds,
    string[] CampaignImageUrls);

public record AdminCollaborationUpsertDto(
    string BrandName,
    string Description,
    string? DescriptionEn,
    string? Testimonial,
    string? TestimonialEn,
    int DisplayOrder,
    bool IsPublished,
    Guid LogoId,
    Guid[] CampaignImageIds);

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

// --- Travel highlights ---

public record AdminHighlightDto(
    Guid Id,
    string Destination,
    string? DestinationEn,
    string Caption,
    string? CaptionEn,
    int DisplayOrder,
    bool IsPublished,
    Guid ImageId,
    string? ImageUrl);

public record AdminHighlightUpsertDto(
    string Destination,
    string? DestinationEn,
    string Caption,
    string? CaptionEn,
    int DisplayOrder,
    bool IsPublished,
    Guid ImageId);

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
    Guid? BackgroundMediaId,
    string? BackgroundMediaUrl);

public record AdminHomeSectionUpdateDto(
    string Headline,
    string? HeadlineEn,
    string Body,
    string? BodyEn,
    string ContentJson,
    string? ContentJsonEn,
    Guid? BackgroundMediaId);
