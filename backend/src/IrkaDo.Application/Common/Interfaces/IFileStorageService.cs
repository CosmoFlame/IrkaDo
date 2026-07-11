namespace IrkaDo.Application.Common.Interfaces;

public record StoredFile(string StorageKey, string PublicUrl);

public record StoredGuideFile(string StorageKey, long SizeBytes);

public interface IFileStorageService
{
    Task<StoredFile> SaveAsync(
        Stream content, string fileName, string contentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a downloadable guide file under the non-web-served guide-files root so it is
    /// only ever reachable through a signed download link, never a static URL.
    /// </summary>
    Task<StoredGuideFile> SaveGuideFileAsync(
        Stream content, string fileName, string contentType, CancellationToken cancellationToken = default);

    string GetPublicUrl(string storageKey);

    Task<Stream> OpenGuideFileAsync(string storageKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Builds a signed, time-limited download link. <paramref name="baseUrlOverride"/> lets a caller
    /// that is serving an HTTP request point the link at the current request's own scheme+host
    /// (robust against a misconfigured API base URL); background callers (e.g. emailed links) leave it
    /// null to fall back to the configured API base URL.
    /// </summary>
    Task<string> GetSignedDownloadUrlAsync(
        string storageKey, string fileName, TimeSpan expiry,
        string? baseUrlOverride = null, CancellationToken cancellationToken = default);
}
