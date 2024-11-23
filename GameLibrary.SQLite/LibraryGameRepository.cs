using System.Runtime.CompilerServices;
using GameLibrary.Core;
using GameLibrary.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLibrary.SQLite;

public class LibraryGameRepository(IDbContextFactory<LibraryContext> libraryContextFactory) : ILibraryGameRepository
{
    public async Task<LibraryGame?> GetLibraryGameByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync(cancellationToken);
        return await libraryContext.LibraryGames
            .Include(libraryGame => libraryGame.Release)
            .ThenInclude(release => release.Game)
            .ThenInclude(game => game.BackgroundImage)
            .Include(libraryGame => libraryGame.Release)
            .ThenInclude(release => release.Game)
            .ThenInclude(game => game.CoverImage)
            .Include(libraryGame => libraryGame.Release)
            .ThenInclude(release => release.Game)
            .ThenInclude(game => game.Developers)
            .Include(libraryGame => libraryGame.Release)
            .ThenInclude(release => release.Game)
            .ThenInclude(game => game.Publishers)
            .Where(g => g.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<LibraryGameSummary>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync(cancellationToken);
        return await libraryContext.LibraryGames
            .Include(libraryGame => libraryGame.Release)
            .ThenInclude(release => release.Game)
            .Include(libraryGame => libraryGame.Release)
            .ThenInclude(release => release.Game)
            .ThenInclude(game => game.CoverImage)
            .Select(g => new LibraryGameSummary
            {
                Id = g.Id,
                LibraryService = g.LibraryService,
                Name = g.Release.Game.Name,
                SortingName = g.Release.Game.GetEffectiveSortingName(),
                Region = g.Release.Region,
                ReleaseDate = g.Release.ReleaseDate,
                ReleaseDatePrecision = g.Release.ReleaseDatePrecision,
                LibraryGameStatus = g.LibraryGameStatus,
                CoverImage = g.Release.Game.CoverImage,
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync()
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        return await libraryContext.Games.CountAsync();
    }

    public async Task DeleteAllAsync()
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        await libraryContext.Games.ExecuteDeleteAsync();
        await libraryContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(LibraryGame existing)
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        libraryContext.LibraryGames.Update(existing);
        await libraryContext.SaveChangesAsync();
    }

    public async Task<LibraryGame?> FindExistingAsync(LibraryGame libraryGame)
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        return await libraryContext.LibraryGames.Where(g =>
                g.Id == libraryGame.Id || (g.LibraryService == libraryGame.LibraryService &&
                                           g.LibraryId == libraryGame.LibraryId &&
                                           g.ExternalId == libraryGame.ExternalId))
            .FirstOrDefaultAsync();
    }

    public async Task Update(Game existing)
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        libraryContext.Update(existing);
        await libraryContext.SaveChangesAsync();
    }
}
