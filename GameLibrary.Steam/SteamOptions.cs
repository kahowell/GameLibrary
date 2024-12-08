using GameLibrary.Core;

namespace GameLibrary.Steam;

public class SteamOptions
{
    public List<SteamLibraryOptions> Libraries { get; set; } = new();
    public bool UseCache { get; set; } = false;

    public string? GetApiKey(string steamId)
    {
        return Libraries.Where(x => x.SteamId == steamId).Select(x => x.ApiKey).FirstOrDefault();
    }
}

public class SteamLibraryOptions
{
    public required string SteamId { get; set; }
    public required string? ApiKey { get; set; }
    public required string DisplayName { get; set; }
}
