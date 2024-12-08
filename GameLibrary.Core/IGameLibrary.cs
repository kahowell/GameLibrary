using GameLibrary.Core.Models;

namespace GameLibrary.Core;

public interface IGameLibrary
{
    string LibraryId { get; }
    string DisplayName { get; }
    Task<IEnumerable<LibraryGame>> GetGamesAsync();
    Task<int> GetGameCountAsync();
}
