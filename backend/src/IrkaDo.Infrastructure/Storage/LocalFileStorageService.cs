using IrkaDo.Application.Common.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace IrkaDo.Infrastructure.Storage;

public class LocalStorageOptions
{
    public string RootPath { get; set; } = "wwwroot/uploads";
    public string PublicBaseUrl { get; set; } = "http://localhost:5000";
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

    public LocalFileStorageService(IHostEnvironment environment, IOptions<LocalStorageOptions> options)
    {
        _environment = environment;
        _options = options.Value;
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
}
