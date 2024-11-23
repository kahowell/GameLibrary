using System.CommandLine;
using GameLibrary.Core.Services;
using GameLibrary.Steam;

namespace GameLibrary.CLI.Commands;

public class SyncCommand: Command
{
    public SyncCommand(SteamSourceProvider steam, LibrarySyncService syncService) : base("sync", "Sync games from game services")
    {
        this.SetHandler(async() =>
        {
            foreach (var account in await steam.GetAvailableLibrariesAsync())
            {
                await syncService.CreateSyncTask(account).Run();
            }
        });
    }
}
