namespace IrkaDo.Application.Features;

/// <summary>
/// A public image reference carrying both its URL and the editor-authored (localized) alt text,
/// so the frontend can render accessible <c>alt</c> attributes instead of falling back to titles.
/// </summary>
public record ImageDto(string Url, string? Alt);
