using GameLibrary.Core.Models;

namespace GameLibrary.Core;

public interface IMetadataRepository
{
    Task UpdateGameAsync(Game game);
    Task UpdateGenreAsync(Genre genre);
    Task UpdateKeywordAsync(Keyword keyword);
    Task UpdateMultiplayerModeAsync(MultiplayerMode multiplayerMode);
    Task UpdateThemeAsync(Theme theme);
    Task<Genre?> FindExistingGenreAsync(Genre genre);
    Task<Keyword?> FindExistingKeywordAsync(Keyword keyword);
    Task<MultiplayerMode?> FindExistingMultiplayerModeAsync(MultiplayerMode multiplayerMode);
    Task<Theme?> FindExistingThemeAsync(Theme theme);
    Task<Game?> FindExistingGameAsync(Game game);
    Task<Platform?> FindExistingPlatformAsync(Platform platform);
    Task UpdatePlatformAsync(Platform platform);
}
