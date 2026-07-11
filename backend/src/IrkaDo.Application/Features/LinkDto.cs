namespace IrkaDo.Application.Features;

/// <summary>
/// A public outbound link with its resolved (localized) display title, attached to a news
/// article, travel guide, or collaboration.
/// </summary>
public record LinkDto(string Url, string? Title);
