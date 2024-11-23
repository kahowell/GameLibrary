using GameLibrary.Core.Models;

namespace GameLibrary.Core;

public interface IGameSourceProvider
{
    LibraryService LibraryService { get; }
    Task<IEnumerable<IGameLibrary>> GetAvailableLibrariesAsync();
}
