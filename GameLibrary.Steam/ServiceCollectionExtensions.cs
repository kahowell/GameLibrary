using System.Diagnostics.CodeAnalysis;
using System.Threading.RateLimiting;
using GameLibrary.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace GameLibrary.Steam;

public static class ServiceCollectionExtensions
{
    [SuppressMessage("SonarLint", "S1075", Justification="I'm hardcoding URLs, anyways")]
    public static IServiceCollection AddSteamServices(this IServiceCollection services)
    {
        Configuration.Register<SteamOptions>();
        services.AddHttpClient<SteamSourceProvider>(configureClient: static client =>
        {
            client.BaseAddress = new Uri("https://api.steampowered.com");
        }).AddResilienceHandler("SteamApi", static builder =>
        {
            builder.AddRetry(new HttpRetryStrategyOptions()
            {
                BackoffType = DelayBackoffType.Exponential,
                MaxRetryAttempts = 3,
                UseJitter = true
            });
            builder.AddRateLimiter(new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions()
            {
                Window = TimeSpan.FromSeconds(1),
                PermitLimit = 3,
                QueueLimit = int.MaxValue,
                AutoReplenishment = true
            }));
        });
        return services;
    }
}
