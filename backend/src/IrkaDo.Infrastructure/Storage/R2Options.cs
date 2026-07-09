namespace IrkaDo.Infrastructure.Storage;

/// <summary>
/// Configuration for the Cloudflare R2 (S3-compatible) storage backend. Bound from the "R2"
/// configuration section; supply via environment variables / secrets in production.
/// </summary>
public class R2Options
{
    /// <summary>R2 S3 API endpoint, e.g. https://&lt;account-id&gt;.r2.cloudflarestorage.com</summary>
    public string ServiceUrl { get; set; } = string.Empty;

    public string AccessKeyId { get; set; } = string.Empty;
    public string SecretAccessKey { get; set; } = string.Empty;

    /// <summary>Bucket for public assets (images/media). Should be exposed via an r2.dev or custom CDN domain.</summary>
    public string PublicBucket { get; set; } = string.Empty;

    /// <summary>Bucket for private guide files. Must NOT have public access — only reachable via signed app links.</summary>
    public string PrivateBucket { get; set; } = string.Empty;

    /// <summary>Public base URL (CDN/custom domain or r2.dev) that serves objects in <see cref="PublicBucket"/>.</summary>
    public string PublicBaseUrl { get; set; } = string.Empty;

    /// <summary>Public base URL of this backend API, used to build /api/v1/downloads/{token} links.</summary>
    public string ApiBaseUrl { get; set; } = string.Empty;
}
