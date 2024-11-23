using System.Diagnostics.CodeAnalysis;
using System.Threading.RateLimiting;
using GameLibrary.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace GameLibrary.IGDB;

public static class ServiceCollectionExtensions
{
    [SuppressMessage("SonarLint", "S1075", Justification="I'm hardcoding URLs, anyways")]
    public static IServiceCollection AddIgdbServices(this IServiceCollection services)
    {
        Configuration.Register<IgdbOptions>();
        services.AddHttpClient<IgdbAuth>(configureClient: static client =>
        {
            client.BaseAddress = new Uri("https://id.twitch.tv");
        }).AddResilienceHandler("TwitchOauth", static builder =>
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
        services.AddHttpClient<IDataSource, IgdbDataSource>(configureClient: static client =>
        {
            client.BaseAddress = new Uri("https://api.igdb.com");
        })
            .AddHttpMessageHandler<IgdbAuth>()
            .AddResilienceHandler("IGDB", static builder =>
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
        services.AddHttpClient<IgdbImageFetcher>(configureClient: static client =>
            {
                client.BaseAddress = new Uri("https://images.igdb.com");
            })
            .AddHttpMessageHandler<IgdbAuth>()
            .AddResilienceHandler("IGDB-images", static builder =>
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
