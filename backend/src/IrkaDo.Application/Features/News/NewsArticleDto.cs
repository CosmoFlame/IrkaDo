namespace IrkaDo.Application.Features.News;

public record NewsArticleSummaryDto(
    string Slug,
    string Title,
    string Excerpt,
    string? CoverImageUrl,
    string? CoverImageAlt,
    DateTimeOffset? PublishedAt,
    int ReadingTimeMinutes,
    string? Category);

public record NewsArticleDetailDto(
    string Slug,
    string Title,
    string Content,
    string? CoverImageUrl,
    string? CoverImageAlt,
    DateTimeOffset? PublishedAt,
    int ReadingTimeMinutes,
    string? Category,
    string[] Tags,
    LinkDto[] Links,
    string? MetaTitle,
    string? MetaDescription,
    string? OgImageUrl);
