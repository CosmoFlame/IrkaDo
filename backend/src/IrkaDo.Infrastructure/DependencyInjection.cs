using IrkaDo.Application.Common.Interfaces;
using IrkaDo.Infrastructure.Auth;
using IrkaDo.Infrastructure.Email;
using IrkaDo.Infrastructure.Payments;
using IrkaDo.Infrastructure.Persistence;
using IrkaDo.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IrkaDo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.Configure<StripeOptions>(configuration.GetSection("Stripe"));
        services.AddScoped<IPaymentProvider, StripePaymentProvider>();

        services.Configure<ResendOptions>(configuration.GetSection("Resend"));
        services.AddHttpClient<IEmailSender, ResendEmailSender>();

        // Post-payment guide delivery runs off a shared in-process queue, drained by a hosted
        // background service so the payment webhook never waits on email delivery.
        services.AddSingleton<GuideDeliveryQueue>();
        services.AddSingleton<IGuideDeliveryQueue>(sp => sp.GetRequiredService<GuideDeliveryQueue>());
        services.AddScoped<IGuideDeliveryService, GuideDeliveryService>();
        services.AddHostedService<GuideDeliveryBackgroundService>();

        services.Configure<LocalStorageOptions>(configuration.GetSection("Storage"));
        services.AddSingleton<IDownloadTokenSigner, HmacDownloadTokenSigner>();

        // Storage backend is selected by config: "R2" for Cloudflare R2 (durable object storage,
        // required for the ephemeral-filesystem container host), otherwise local disk for dev.
        var storageProvider = configuration["Storage:Provider"] ?? "Local";
        if (string.Equals(storageProvider, "R2", StringComparison.OrdinalIgnoreCase))
        {
            var r2Options = configuration.GetSection("R2").Get<R2Options>() ?? new R2Options();
            services.AddSingleton(r2Options);
            services.AddSingleton<IFileStorageService, R2FileStorageService>();
        }
        else
        {
            services.AddSingleton<IFileStorageService, LocalFileStorageService>();
        }

        services.Configure<AdminOptions>(configuration.GetSection("Admin"));
        services.AddSingleton<IAdminTokenService, AdminTokenService>();

        return services;
    }
}
