using GameLibrary.Core.Models;

namespace GameLibrary.Core.Mocks;

public class MockPlatformRepository: IPlatformRepository
{
    private readonly IList<Platform> _platforms =
    [
        new() { Name = "PC" }
    ];

    public Task<IEnumerable<Platform>> GetAll()
    {
        return Task.FromResult(_platforms.AsEnumerable());
    }

    public Task Add(Platform platform) => Task.CompletedTask;

    public Task<int> Count() => Task.FromResult(_platforms.Count);

    public Task DeleteAll() => Task.CompletedTask;

    public Task<bool> ExistsByIgdbId(ulong igdbId) => Task.FromResult(true);

    public Task<Platform?> GetByIgdbId(ulong igdbId) => Task.FromResult(_platforms.FirstOrDefault(p => p.IgdbId == igdbId));
}
