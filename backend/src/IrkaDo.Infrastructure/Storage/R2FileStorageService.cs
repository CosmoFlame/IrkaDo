using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using IrkaDo.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace IrkaDo.Infrastructure.Storage;

/// <summary>
/// Cloudflare R2 (S3-compatible) implementation of <see cref="IFileStorageService"/>.
///
/// Public images live in a CDN-fronted public bucket; downloadable guide files live in a private
/// bucket and are never exposed directly. Guide downloads keep flowing through the app's signed
/// <c>/api/v1/downloads/{token}</c> endpoint (which enforces the HMAC token, rate limiting, and
/// download logging) — <see cref="OpenGuideFileAsync"/> just streams the bytes out of R2.
/// </summary>
public class R2FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3;
    private readonly R2Options _options;
    private readonly IDownloadTokenSigner _tokenSigner;

    public R2FileStorageService(R2Options options, IDownloadTokenSigner tokenSigner)
    {
        _options = options;
        _tokenSigner = tokenSigner;

        var config = new AmazonS3Config
        {
            ServiceURL = options.ServiceUrl,
            ForcePathStyle = true,
            // R2 rejects the AWS SDK's default per-request integrity checksums; only send them when
            // an operation actually requires one.
            RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
            ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED,
        };
        _s3 = new AmazonS3Client(
            new BasicAWSCredentials(options.AccessKeyId, options.SecretAccessKey), config);
    }

    public async Task<StoredFile> SaveAsync(
        Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var storageKey = $"media/{Guid.NewGuid()}-{SanitizeFileName(fileName)}";
        await PutAsync(_options.PublicBucket, storageKey, content, contentType, cancellationToken);
        return new StoredFile(storageKey, GetPublicUrl(storageKey));
    }

    public async Task<StoredGuideFile> SaveGuideFileAsync(
        Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var storageKey = $"{Guid.NewGuid()}-{SanitizeFileName(fileName)}";
        var sizeBytes = content.CanSeek ? content.Length : 0;
        await PutAsync(_options.PrivateBucket, storageKey, content, contentType, cancellationToken);

        // Fall back to a HEAD if the source stream wasn't seekable so we still record an accurate size.
        if (sizeBytes == 0)
        {
            var meta = await _s3.GetObjectMetadataAsync(_options.PrivateBucket, storageKey, cancellationToken);
            sizeBytes = meta.ContentLength;
        }

        return new StoredGuideFile(storageKey, sizeBytes);
    }

    public string GetPublicUrl(string storageKey) =>
        $"{_options.PublicBaseUrl.TrimEnd('/')}/{storageKey.TrimStart('/')}";

    public async Task<Stream> OpenGuideFileAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _s3.GetObjectAsync(_options.PrivateBucket, storageKey, cancellationToken);
            return response.ResponseStream;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Mirror the local provider so DownloadsController maps a missing object to a 404.
            throw new FileNotFoundException($"Guide file '{storageKey}' was not found in storage.", ex);
        }
    }

    public Task<string> GetSignedDownloadUrlAsync(
        string storageKey, string fileName, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        // The link points back at this API, not R2 directly, so the existing HMAC token + rate limiting
        // + download logging still gate every fetch.
        var token = _tokenSigner.CreateToken(storageKey, fileName, expiry);
        return Task.FromResult($"{_options.ApiBaseUrl.TrimEnd('/')}/api/v1/downloads/{token}");
    }

    private async Task PutAsync(
        string bucket, string key, Stream content, string contentType, CancellationToken cancellationToken)
    {
        var request = new PutObjectRequest
        {
            BucketName = bucket,
            Key = key,
            InputStream = content,
            ContentType = contentType,
            AutoCloseStream = false,
            // R2 doesn't implement the AWS SDK's default chunked payload signing
            // (STREAMING-AWS4-HMAC-SHA256-PAYLOAD). Send an UNSIGNED-PAYLOAD instead — safe over the
            // HTTPS endpoint, and accepted by R2.
            DisablePayloadSigning = true,
        };
        await _s3.PutObjectAsync(request, cancellationToken);
    }

    private static string SanitizeFileName(string fileName)
    {
        var name = Path.GetFileName(fileName);
        return string.IsNullOrWhiteSpace(name) ? "file" : name;
    }
}
