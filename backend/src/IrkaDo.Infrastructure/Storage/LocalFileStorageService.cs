using IrkaDo.Application.Common.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace IrkaDo.Infrastructure.Storage;

public class LocalStorageOptions
{
    public string RootPath { get; set; } = "wwwroot/uploads";
    public string PublicBaseUrl { get; set; } = "http://localhost:5000";

    /// <summary>
    /// Root for guide files (PDFs, etc). Deliberately a sibling of wwwroot, not under it, so
    /// <c>UseStaticFiles()</c> never serves these directly — access is only via signed download tokens.
    /// </summary>
    public string GuideFilesRootPath { get; set; } = "GuideFiles";

    public string SigningKey { get; set; } = "dev-signing-key-change-me";
}

/// <summary>
/// Writes files to disk under the API project and serves them via ASP.NET static file middleware.
/// Implements <see cref="IFileStorageService"/> so it can be swapped for an S3/Azure Blob/R2-backed
/// implementation later without changing any calling code.
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly IHostEnvironment _environment;
    private readonly LocalStorageOptions _options;
    private readonly IDownloadTokenSigner _tokenSigner;

    public LocalFileStorageService(
        IHostEnvironment environment, IOptions<LocalStorageOptions> options, IDownloadTokenSigner tokenSigner)
    {
        _environment = environment;
        _options = options.Value;
        _tokenSigner = tokenSigner;
    }

    public async Task<StoredFile> SaveAsync(
        Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var storageKey = $"{Guid.NewGuid()}-{fileName}";
        var fullPath = Path.Combine(_environment.ContentRootPath, _options.RootPath, storageKey);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var fileStream = File.Create(fullPath);
        await content.CopyToAsync(fileStream, cancellationToken);

        return new StoredFile(storageKey, GetPublicUrl(storageKey));
    }

    public string GetPublicUrl(string storageKey) =>
        $"{_options.PublicBaseUrl.TrimEnd('/')}/uploads/{storageKey.TrimStart('/')}";

    public Task<Stream> OpenGuideFileAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_environment.ContentRootPath, _options.GuideFilesRootPath, storageKey);
        Stream stream = File.OpenRead(fullPath);
        return Task.FromResult(stream);
    }

    public Task<string> GetSignedDownloadUrlAsync(
        string storageKey, string fileName, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        var token = _tokenSigner.CreateToken(storageKey, fileName, expiry);
        return Task.FromResult($"{_options.PublicBaseUrl.TrimEnd('/')}/api/v1/downloads/{token}");
    }
}
