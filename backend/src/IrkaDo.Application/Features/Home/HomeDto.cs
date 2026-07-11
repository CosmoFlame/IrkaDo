using IrkaDo.Application.Features.Guides;
using IrkaDo.Application.Features.News;

namespace IrkaDo.Application.Features.Home;

public record HeroDto(string Headline, string Body, string? BackgroundMediaUrl);
public record AboutDto(string Headline, string Body);
public record ContactDto(string Headline, string Body, string? Email);

public record SocialLinkDto(string Platform, string Url, string? Description, int? FollowerCount);

public record CollaborationDto(
    string BrandName, string Description, string? Testimonial,
    string? CoverImageUrl, string? CoverImageAlt, LinkDto[] Links);

public record HomePageDto(
    HeroDto Hero,
    AboutDto About,
    ContactDto Contact,
    SocialLinkDto[] SocialLinks,
    CollaborationDto[] Collaborations,
    TravelGuideSummaryDto[] FeaturedGuides,
    NewsArticleSummaryDto[] LatestNews);
