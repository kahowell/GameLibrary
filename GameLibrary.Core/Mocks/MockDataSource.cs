using GameLibrary.Core.Models;

namespace GameLibrary.Core.Mocks;

public class MockDataSource: IDataSource
{
    private readonly IList<Platform> _platforms =
    [
        new() { Name = "PC" }
    ];

    public Task<IEnumerable<Platform>> GetAllPlatforms()
    {
        return Task.FromResult(_platforms.AsEnumerable());
    }

    public Task<Game?> FindGameAsync(LibraryGame libraryGame)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> FetchImageAsync(Image image)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> FetchImage(string imageId)
    {
        throw new NotImplementedException();
    }

    public Task<Game?> GetGameDetails(Game game)
    {
        return Task.FromResult(game)!;
    }

    public Task<byte[]> FetchImage(Image image)
    {
        return Task.FromResult(Array.Empty<byte>());
    }
}
