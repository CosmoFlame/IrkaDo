namespace IrkaDo.Application.Common.Interfaces;

public record StoredFile(string StorageKey, string PublicUrl);

public interface IFileStorageService
{
    Task<StoredFile> SaveAsync(
        Stream content, string fileName, string contentType, CancellationToken cancellationToken = default);

    string GetPublicUrl(string storageKey);
}
