using IrkaDo.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace IrkaDo.Api.Controllers;

[ApiController]
[Route("api/v1/downloads")]
public class DownloadsController : ControllerBase
{
    private readonly IDownloadTokenSigner _tokenSigner;
    private readonly IFileStorageService _storage;

    public DownloadsController(IDownloadTokenSigner tokenSigner, IFileStorageService storage)
    {
        _tokenSigner = tokenSigner;
        _storage = storage;
    }

    [HttpGet("{token}")]
    [EnableRateLimiting("download")]
    public async Task<IActionResult> Get(string token, CancellationToken cancellationToken)
    {
        if (!_tokenSigner.TryValidate(token, out var storageKey, out var fileName))
            return Unauthorized("This download link is invalid or has expired.");

        Stream stream;
        try
        {
            stream = await _storage.OpenGuideFileAsync(storageKey, cancellationToken);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }

        return File(stream, "application/octet-stream", fileName);
    }
}
