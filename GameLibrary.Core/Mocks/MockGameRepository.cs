using GameLibrary.Core.Models;

namespace GameLibrary.Core.Mocks;

public class MockGameRepository: ILibraryGameRepository
{
    public Task<LibraryGame> GetLibraryGameByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task<IEnumerable<LibraryGameSummary>> ILibraryGameRepository.GetAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<LibraryGame>> GetAllAsync(string? searchText, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(LibraryGame game)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync()
    {
        throw new NotImplementedException();
    }

    public Task DeleteAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(LibraryGame existing)
    {
        throw new NotImplementedException();
    }

    public Task<LibraryGame?> FindExistingAsync(LibraryGame libraryGame)
    {
        throw new NotImplementedException();
    }
}
