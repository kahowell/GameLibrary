using GameLibrary.Core.Models;

namespace GameLibrary.Core;

public interface IDataSource
{
    Task<IEnumerable<Platform>> GetAllPlatforms();
    Task<Game?> FindGameAsync(LibraryGame libraryGame);
    Task<Stream> FetchImageAsync(Image image);
}
