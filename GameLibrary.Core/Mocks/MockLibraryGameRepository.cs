using GameLibrary.Core.Models;

namespace GameLibrary.Core.Mocks;

public class MockLibraryGameRepository: ILibraryGameRepository
{
    private readonly IList<LibraryGame> _games = [
        new()
        {
            LibraryId = null,
            ExternalId = null,
            LibraryGameStatus = LibraryGameStatus.Available
        }
    ];

    public Task<LibraryGame> GetLibraryGameByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<LibraryGameSummary>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync() => Task.FromResult(_games.Count);

    public Task DeleteAllAsync() => Task.CompletedTask;

    public Task UpdateAsync(LibraryGame existing) => Task.CompletedTask;

    public Task<LibraryGame?> FindExistingAsync(LibraryGame libraryGame) => Task.FromResult(null as LibraryGame);
}
