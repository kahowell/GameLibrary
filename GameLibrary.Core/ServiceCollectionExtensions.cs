using GameLibrary.Core.Mocks;
using GameLibrary.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GameLibrary.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameLibraryCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<Configuration>();
        services.AddSingleton<LibrarySyncService>();
        return services;
    }

    public static IServiceCollection AddMockGameLibraryServices(this IServiceCollection services)
    {
        services.AddSingleton<ICompanyRepository, MockCompanyRepository>();
        services.AddSingleton<IDataSource, MockDataSource>();
        services.AddSingleton<IGameLibrary, MockGameLibrary>();
        services.AddSingleton<ILibraryGameRepository, MockLibraryGameRepository>();
        services.AddSingleton<IImageRepository, MockImageRepository>();
        services.AddSingleton<IPlatformRepository, MockPlatformRepository>();
        return services;
    }
}
