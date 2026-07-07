namespace IrkaDo.Api.Localization;

/// <summary>
/// Resolves the requested content language from the <c>?lang</c> query string.
/// Ukrainian is the default; only an explicit <c>en</c> switches to English.
/// Public read controllers use the returned flag to pick between the base
/// (Ukrainian) columns and their <c>*En</c> siblings, falling back to the base
/// value when an English translation is missing.
/// </summary>
public static class RequestLanguage
{
    public const string QueryKey = "lang";
    public const string English = "en";

    /// <summary>True when the caller explicitly asked for English content.</summary>
    public static bool IsEnglish(this HttpRequest request) =>
        string.Equals(request.Query[QueryKey], English, StringComparison.OrdinalIgnoreCase);
}
