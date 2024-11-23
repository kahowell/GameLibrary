using System.Diagnostics.CodeAnalysis;
using GameLibrary.Core;
using Microsoft.Extensions.DependencyInjection;

namespace GameLibrary.SQLite;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqliteServices(this IServiceCollection services)
    {
        services.AddDbContextFactory<LibraryContext>();
        services.AddSingleton<ILibraryGameRepository, LibraryGameRepository>();
        services.AddSingleton<IPlatformRepository, PlatformRepository>();
        services.AddSingleton<IImageRepository, ImageRepository>();
        services.AddSingleton<ICompanyRepository, CompanyRepository>();
        services.AddSingleton<IMetadataRepository, MetadataRepository>();
        return services;
    }
}
