using System.Text.Json;
using System.Text.RegularExpressions;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.Steam;
using GameFinder.StoreHandlers.Steam.Models;
using GameFinder.StoreHandlers.Steam.Services;
using GameLibrary.Core;
using GameLibrary.Core.Models;
using NexusMods.Paths;

namespace GameLibrary.Steam;

public class SteamSourceProvider(HttpClient client, Configuration configuration): IGameSourceProvider
{
    public LibraryService LibraryService => LibraryService.Steam;

    public Task<IEnumerable<IGameLibrary>> GetAvailableLibrariesAsync()
    {
        var profiles = SteamLocationFinder
            .FindSteam(FileSystem.Shared, OperatingSystem.IsWindows() ? WindowsRegistry.Shared : null).Value
            .Combine(SteamLocationFinder.UserDataDirectoryName).EnumerateDirectories(recursive: false);
        return Task.FromResult<IEnumerable<IGameLibrary>>(profiles.Select(directory =>
        {
            var steamId = SteamId.FromAccountId(UInt32.Parse(directory.Name));
            // NOTE: above factory method doesn't set account instance to 1. See https://steamcommunity.com/profiles/
            steamId = SteamId.From(steamId.RawId | 0x100000000);
            return new SteamLibrary(client, configuration, steamId.RawId.ToString());
        }));
    }
}

public partial class SteamLibrary(HttpClient client, Configuration configuration, string steamId) : IGameLibrary
{
    [GeneratedRegex("^Steam Linux Runtime")]
    private static partial Regex SteamLinuxRuntimeRegex();
    [GeneratedRegex("^Steamworks Common Redistributables")]
    private static partial Regex SteamCommonRedistributablesRegex();
    [GeneratedRegex("^Proton (Experimental|Hotfix|[0-9])")]
    private static partial Regex ProtonRegex();
    private static readonly IEnumerable<Regex> RegexDenylist = [
        SteamLinuxRuntimeRegex(),
        SteamCommonRedistributablesRegex(),
        ProtonRegex()
    ];
    private readonly SteamHandler _handler = new(FileSystem.Shared, OperatingSystem.IsWindows() ? WindowsRegistry.Shared : null);
    private readonly string _apiKey = configuration.Get<SteamOptions>().ApiKey;
    private readonly bool _useCache = configuration.Get<SteamOptions>().UseCache;
    private readonly string _cacheDir = Configuration.EnsureCacheDirectory("GameLibrary.Steam");

    private async Task<SteamUserGameReport> GetSteamUserGameReportAsync()
    {
        string? cache = null;
        if (_useCache)
        {
            Console.WriteLine("Using cache for steam user");
            cache = Path.Join(_cacheDir, $"{steamId}.json");
            if (Path.Exists(cache))
            {
                return await JsonSerializer.DeserializeAsync<SteamUserGameReport>(new FileStream(cache, FileMode.Open));
            }
        }
        var result = await client.GetAsync($"IPlayerService/GetOwnedGames/v0001/?key={_apiKey}&steamid={steamId}&format=json");
        var content = await result.Content.ReadAsByteArrayAsync();
        if (cache != null) await new FileStream(cache, FileMode.Create).WriteAsync(content.AsMemory());
        return await JsonSerializer.DeserializeAsync<SteamUserGameReport>(new MemoryStream(content));
    }

    private async IAsyncEnumerable<uint> GetInstalledAppIdsAsync()
    {
        foreach (var result in await Task.Run(_handler.FindAllGames))
        {
            var game = result.Match(game => game, error =>
            {
                Console.WriteLine("Got an error!");
                Console.WriteLine(error.Message);
                return null!;
            });
            if (game == null) continue;
            if (RegexDenylist.Any(regex => regex.IsMatch(game.Name))) continue;
            yield return game.AppId.Value;
        }
    }

    public string LibraryId => steamId;

    public async Task<IEnumerable<LibraryGame>> GetGamesAsync()
    {
        var availableGames = await GetSteamUserGameReportAsync();
        var installedGames = GetInstalledAppIdsAsync();
        var installedAppIds = new HashSet<uint>();
        await foreach (var appId in installedGames)
        {
            installedAppIds.Add(appId);
        }

        return availableGames.Response.Games
            .Select(game => game.AppId)
            .Select(appId =>
            {
                var status = installedAppIds.Contains((uint)appId)
                    ? LibraryGameStatus.Installed
                    : LibraryGameStatus.Available;
                return new LibraryGame
                {
                    LibraryId = steamId,
                    LibraryService = LibraryService.Steam,
                    ExternalId = appId.ToString(),
                    LibraryGameStatus = status
                };
            });
    }

    public async Task<int> GetGameCountAsync()
    {
        var availableGames = await GetSteamUserGameReportAsync();
        return availableGames.Response.GameCount;
    }
}
