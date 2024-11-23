using GameLibrary.Core;
using GameLibrary.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLibrary.SQLite;

public class PlatformRepository(IDbContextFactory<LibraryContext> libraryContextFactory): IPlatformRepository
{
    public async Task<IEnumerable<Platform>> GetAll()
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        return libraryContext.Platforms;
    }

    public async Task Add(Platform platform)
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        await libraryContext.Platforms.AddAsync(platform);
        await libraryContext.SaveChangesAsync();
    }

    public async Task<int> Count()
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        return await libraryContext.Platforms.CountAsync();
    }

    public async Task DeleteAll()
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        await libraryContext.Platforms.ExecuteDeleteAsync();
        await libraryContext.SaveChangesAsync();
    }

    public Task<bool> ExistsByIgdbId(ulong igdbId)
    {
        throw new NotImplementedException();
    }

    public Task<Platform?> GetByIgdbId(ulong igdbId)
    {
        throw new NotImplementedException();
    }
}
