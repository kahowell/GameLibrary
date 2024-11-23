using GameLibrary.Core.Models;

namespace GameLibrary.Core;

public interface IPlatformRepository
{
    Task<IEnumerable<Platform>> GetAll();
    Task Add(Platform platform);
    Task<int> Count();
    Task DeleteAll();
    Task<bool> ExistsByIgdbId(ulong igdbId);
    Task<Platform?> GetByIgdbId(ulong igdbId);
}
