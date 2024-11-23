using GameLibrary.Core;
using GameLibrary.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLibrary.SQLite;

public class MetadataRepository(IDbContextFactory<LibraryContext> libraryContextFactory): IMetadataRepository
{
    public async Task UpdateGameAsync(Game game)
    {
        var context = await libraryContextFactory.CreateDbContextAsync();
        context.Games.Update(game);
        await context.SaveChangesAsync();
    }

    public async Task UpdateGenreAsync(Genre genre)
    {
        var context = await libraryContextFactory.CreateDbContextAsync();
        context.Genres.Update(genre);
        await context.SaveChangesAsync();
    }

    public async Task UpdateKeywordAsync(Keyword keyword)
    {
        var context = await libraryContextFactory.CreateDbContextAsync();
        context.Keywords.Update(keyword);
        await context.SaveChangesAsync();
    }

    public async Task UpdateMultiplayerModeAsync(MultiplayerMode multiplayerMode)
    {
        var context = await libraryContextFactory.CreateDbContextAsync();
        context.MultiplayerModes.Update(multiplayerMode);
        await context.SaveChangesAsync();
    }

    public async Task UpdateThemeAsync(Theme theme)
    {
        var context = await libraryContextFactory.CreateDbContextAsync();
        context.Themes.Update(theme);
        await context.SaveChangesAsync();
    }

    public async Task<Genre?> FindExistingGenreAsync(Genre genre)
    {
        var context = await libraryContextFactory.CreateDbContextAsync();
        return await context.Genres
            .Where(g => g.Id == genre.Id || (g.IgdbId != null && genre.IgdbId != null &&
                                             g.IgdbId == genre.IgdbId)).FirstOrDefaultAsync();
    }

    public async Task<Keyword?> FindExistingKeywordAsync(Keyword keyword)
    {
        var context = await libraryContextFactory.CreateDbContextAsync();
        return await context.Keywords
            .Where(k => k.Id == keyword.Id || (k.IgdbId != null && keyword.IgdbId != null &&
                                             k.IgdbId == keyword.IgdbId)).FirstOrDefaultAsync();
    }

    public async Task<MultiplayerMode?> FindExistingMultiplayerModeAsync(MultiplayerMode multiplayerMode)
    {
        var context = await libraryContextFactory.CreateDbContextAsync();
        return await context.MultiplayerModes
            .Where(m => m.Id == multiplayerMode.Id || (m.IgdbId != null && multiplayerMode.IgdbId != null &&
                                                       m.IgdbId == multiplayerMode.IgdbId))
            .FirstOrDefaultAsync();
    }

    public async Task<Theme?> FindExistingThemeAsync(Theme theme)
    {
        var context = await libraryContextFactory.CreateDbContextAsync();
        return await context.Themes
            .Where(t => t.Id == theme.Id || (t.IgdbId != null && theme.IgdbId != null &&
                                             t.IgdbId == theme.IgdbId)).FirstOrDefaultAsync();
    }

    public async Task<Game?> FindExistingGameAsync(Game game)
    {
        var context = await libraryContextFactory.CreateDbContextAsync();
        return await context.Games
            .Include(g => g.Releases)
                .ThenInclude(r => r.Platform)
            .Include(g => g.Developers)
            .Include(g => g.Publishers)
            .Where(g => g.Id == game.Id || (g.IgdbId != null && game.IgdbId != null &&
                                             g.IgdbId == game.IgdbId)).FirstOrDefaultAsync();
    }

    public async Task<Platform?> FindExistingPlatformAsync(Platform platform)
    {
        var context = await libraryContextFactory.CreateDbContextAsync();
        return await context.Platforms
            .Where(p => p.Id == platform.Id ||
                        (p.IgdbId != null && platform.IgdbId != null && p.IgdbId == platform.IgdbId))
            .FirstOrDefaultAsync();
    }

    public async Task UpdatePlatformAsync(Platform platform)
    {
        var context = await libraryContextFactory.CreateDbContextAsync();
        context.Platforms.Update(platform);
        await context.SaveChangesAsync();
    }
}
