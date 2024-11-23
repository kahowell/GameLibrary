using System.CommandLine;
using GameLibrary.PCGamingWiki;

namespace GameLibrary.CLI.Commands;

public class PcGamingWikiCommand: Command
{
    public PcGamingWikiCommand(PcGamingWikiService wiki) : base("pcgw", "Lookup data via PCGamingWiki")
    {
        var steamAppIdArg = new Argument<string>(name: "steamAppId", description: "Steam AppId");
        AddArgument(steamAppIdArg);
        this.SetHandler(async steamAppId =>
        {
            var id = await wiki.GetIgdbSlugForSteamAppIdAsync(steamAppId);
            Console.WriteLine($"ID-yay: {id}");
        }, steamAppIdArg);
    }
}
