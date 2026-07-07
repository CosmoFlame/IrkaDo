using IrkaDo.Domain.Entities;

namespace IrkaDo.Application.Features.Admin;

// --- Categories ---

public record AdminCategoryDto(Guid Id, string Name, string Slug);

public record AdminCategoryUpsertDto(string Name, string Slug);

// --- Tags ---

public record AdminTagDto(Guid Id, string Name, string Slug);

public record AdminTagUpsertDto(string Name, string Slug);

// --- Pages (SEO metadata per page) ---

public record AdminPageDto(
    Guid Id,
    string Slug,
    string Title,
    string? MetaTitle,
    string? MetaDescription,
    string? OgImageUrl);

public record AdminPageUpsertDto(
    string Slug,
    string Title,
    string? MetaTitle,
    string? MetaDescription,
    string? OgImageUrl);

// --- Media assets ---

public record AdminMediaDto(
    Guid Id,
    string Url,
    MediaAssetType Type,
    string? AltText,
    int? Width,
    int? Height,
    DateTimeOffset CreatedAt);

public record AdminMediaUpdateDto(string? AltText);
