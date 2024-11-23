using System.Diagnostics.CodeAnalysis;
using System.Threading.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace GameLibrary.PCGamingWiki;

public static class ServiceCollectionExtensions
{
    [SuppressMessage("SonarLint", "S1075", Justification="I'm hardcoding URLs, anyways")]
    public static IServiceCollection AddPcGamingWikiService(this IServiceCollection services)
    {
        services.AddHttpClient<PcGamingWikiService>(configureClient: static client =>
        {
            client.BaseAddress = new Uri("https://pcgamingwiki.com");
        }).AddResilienceHandler("PCGamingWikiApi", static builder =>
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
