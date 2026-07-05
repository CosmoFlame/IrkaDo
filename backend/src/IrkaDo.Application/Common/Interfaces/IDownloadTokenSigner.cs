namespace IrkaDo.Application.Common.Interfaces;

public interface IDownloadTokenSigner
{
    string CreateToken(string storageKey, string fileName, TimeSpan expiry);

    bool TryValidate(string token, out string storageKey, out string fileName);
}
