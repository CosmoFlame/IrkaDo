using System.Net.Http.Headers;
using System.Net.Http.Json;
using IrkaDo.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace IrkaDo.Infrastructure.Email;

public class ResendOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string FromAddress { get; set; } = "IrkaDo <hello@irkado.com>";
}

public class ResendEmailSender : IEmailSender
{
    private readonly HttpClient _httpClient;
    private readonly ResendOptions _options;

    public ResendEmailSender(HttpClient httpClient, IOptions<ResendOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _httpClient.BaseAddress = new Uri("https://api.resend.com/");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.ApiKey);
    }

    public async Task SendAsync(
        string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("emails", new
        {
            from = _options.FromAddress,
            to = new[] { toEmail },
            subject,
            html = htmlBody
        }, cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}
