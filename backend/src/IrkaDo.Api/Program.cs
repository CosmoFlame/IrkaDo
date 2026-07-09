using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Infrastructure;
using IrkaDo.Infrastructure.Persistence;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("download", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));

    // Throttle admin password attempts to slow brute-force guessing.
    options.AddPolicy("login", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));
});

const string FrontendCorsPolicy = "FrontendCorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                      ?? ["http://localhost:3000"];
        policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
    });
});

// Fail fast if security-critical secrets are missing or left at their well-known dev defaults
// outside development, rather than silently running with forgeable tokens/download links.
if (!builder.Environment.IsDevelopment())
{
    var problems = new List<string>();
    if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("Default")))
        problems.Add("ConnectionStrings:Default is not configured.");
    if (string.IsNullOrWhiteSpace(builder.Configuration["Admin:Password"]))
        problems.Add("Admin:Password is not configured.");

    var adminKey = builder.Configuration["Admin:TokenSigningKey"];
    if (string.IsNullOrWhiteSpace(adminKey) || adminKey == "dev-admin-signing-key-change-me")
        problems.Add("Admin:TokenSigningKey must be overridden with a non-default value.");

    var storageKey = builder.Configuration["Storage:SigningKey"];
    if (string.IsNullOrWhiteSpace(storageKey) || storageKey == "dev-signing-key-change-me")
        problems.Add("Storage:SigningKey must be overridden with a non-default value.");

    // When R2 is selected, require its full config — a half-configured provider would crash at the
    // first upload/download anyway, so fail fast here instead.
    var storageProvider = builder.Configuration["Storage:Provider"];
    if (string.Equals(storageProvider, "R2", StringComparison.OrdinalIgnoreCase))
    {
        foreach (var key in new[]
                 {
                     "R2:ServiceUrl", "R2:AccessKeyId", "R2:SecretAccessKey",
                     "R2:PublicBucket", "R2:PrivateBucket", "R2:PublicBaseUrl", "R2:ApiBaseUrl"
                 })
        {
            if (string.IsNullOrWhiteSpace(builder.Configuration[key]))
                problems.Add($"{key} must be configured when Storage:Provider is 'R2'.");
        }
    }

    if (problems.Count > 0)
        throw new InvalidOperationException(
            "Refusing to start with insecure configuration:" + Environment.NewLine +
            " - " + string.Join(Environment.NewLine + " - ", problems));

    // Local disk storage is not durable on an ephemeral-filesystem host (uploads and purchased guide
    // files are lost on redeploy). This is a durability warning, not a security failure — the app
    // still boots so it can run before R2 is set up. Set Storage:Provider=R2 for production.
    if (!string.Equals(storageProvider, "R2", StringComparison.OrdinalIgnoreCase))
        Console.Error.WriteLine(
            "WARNING: Storage:Provider is not 'R2'. Using non-durable local disk storage — uploaded " +
            "images and purchased guide files will be LOST on every redeploy. Set Storage:Provider=R2.");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Apply pending EF migrations on startup in every environment so a deploy self-updates the schema
// (the app runs single-instance). Placeholder seeding stays development-only.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    if (app.Environment.IsDevelopment())
    {
        var storage = scope.ServiceProvider.GetRequiredService<IFileStorageService>();
        await DbSeeder.SeedAsync(db, storage);
    }
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors(FrontendCorsPolicy);

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.Run();
