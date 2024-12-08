using GameLibrary.Core.Models;

namespace GameLibrary.Core.Mocks;

public class MockGameLibrary: IGameLibrary
{
    private readonly IList<LibraryGame> _games =
    [
        new()
        {
            Release = new()
                {
                    Platform = null,
                },
            LibraryId = null,
            ExternalId = null,
            LibraryGameStatus = LibraryGameStatus.Available
        },
        new()
        {
            Release = new()
            {
                Platform = null,
            },
            LibraryId = null,
            ExternalId = null,
            LibraryGameStatus = LibraryGameStatus.Available
        },
        new()
        {
            Release = new()
            {
                Platform = null,
            },
            LibraryId = null,
            ExternalId = null,
            LibraryGameStatus = LibraryGameStatus.Available
        },
    ];

    public string LibraryId => "MockLibrary";
    public string DisplayName => "Mock Library Display Name";
    public Task<IEnumerable<LibraryGame>> GetGamesAsync()
    {
        return Task.FromResult(_games.AsEnumerable());
    }

    public Task<int> GetGameCountAsync() => Task.FromResult(_games.Count);
}
