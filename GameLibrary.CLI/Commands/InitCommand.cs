using System.CommandLine;
using GameLibrary.Core;
using GameLibrary.IGDB;

namespace GameLibrary.CLI.Commands;

internal class InitCommand : Command
{
    public InitCommand(IDataSource dataSource, IPlatformRepository repo) : base("init", "Initialize game library database")
    {
        this.SetHandler(async () =>
        {
            await repo.DeleteAll();
            foreach (var platform in await dataSource.GetAllPlatforms())
            {
                await repo.Add(platform);
            }
        });
    }
}
