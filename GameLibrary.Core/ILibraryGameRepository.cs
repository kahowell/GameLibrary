using GameLibrary.Core.Models;

namespace GameLibrary.Core;

public interface ILibraryGameRepository
{
    Task<LibraryGame?> GetLibraryGameByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LibraryGameSummary>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync();
    Task DeleteAllAsync();
    Task UpdateAsync(LibraryGame existing);
    Task<LibraryGame?> FindExistingAsync(LibraryGame libraryGame);
}
