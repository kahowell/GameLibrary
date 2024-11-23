using GameLibrary.Core.Models;

namespace GameLibrary.Core.Mocks;

public class MockMetadataRepository: IMetadataRepository
{
    public Task UpdateGameAsync(Game game) => Task.CompletedTask;

    public Task UpdateGenreAsync(Genre genre) => Task.CompletedTask;

    public Task UpdateKeywordAsync(Keyword keyword) => Task.CompletedTask;

    public Task UpdateMultiplayerModeAsync(MultiplayerMode multiplayerMode) => Task.CompletedTask;

    public Task UpdateThemeAsync(Theme theme) => Task.CompletedTask;

    public Task<Genre?> FindExistingGenreAsync(Genre genre) => Task.FromResult<Genre?>(null);

    public Task<Keyword?> FindExistingKeywordAsync(Keyword keyword) => Task.FromResult<Keyword?>(null);

    public Task<MultiplayerMode?> FindExistingMultiplayerModeAsync(MultiplayerMode multiplayerMode) => Task.FromResult<MultiplayerMode?>(null);

    public Task<Theme?> FindExistingThemeAsync(Theme theme) => Task.FromResult<Theme?>(null);

    public Task<Game?> FindExistingGameAsync(Game game) => Task.FromResult<Game?>(null);

    public Task<Platform?> FindExistingPlatformAsync(Platform platform) => Task.FromResult<Platform?>(null);

    public Task UpdatePlatformAsync(Platform platform) => Task.CompletedTask;
}
