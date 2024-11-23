using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace GameLibrary.PCGamingWiki;

public class PcGamingWikiService(HttpClient client)
{
    private readonly Regex IgdbIdRegex = new("^\\|igdb *= (.*)$", RegexOptions.Multiline);
    public async Task<string?> GetIgdbSlugForSteamAppIdAsync(string appId)
    {
        var pageId = await GetWikiPageIdForSteamAppIdAsync(appId);
        if (pageId == null) return null;
        var wikiContent = await GetPageContentAsync(pageId);
        Console.WriteLine(wikiContent);
        var match = IgdbIdRegex.Match(wikiContent);
        return match.Groups[1].Value;
    }

    private async Task<string?> GetWikiPageIdForSteamAppIdAsync(string appId)
    {
        var response = await client.GetAsync(
                $"https://www.pcgamingwiki.com/w/api.php?action=cargoquery&tables=Infobox_game&fields=Infobox_game._pageID=PageID&where=Infobox_game.Steam_AppID%20HOLDS%20%22{appId}%22&format=json");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        if (content.RootElement.GetProperty("cargoquery").GetArrayLength() == 0)
        {
            return null;
        }
        return content.RootElement.GetProperty("cargoquery")[0].GetProperty("title").GetProperty("PageID").GetString();
    }

    private async Task<string> GetPageContentAsync(string pageId)
    {
        var response =
            await client.GetAsync(
                $"w/api.php?action=query&format=json&prop=revisions&pageids={pageId}&rvprop=content&rvslots=main");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<JsonDocument>();
        return content.RootElement.GetProperty("query").GetProperty("pages").GetProperty(pageId).GetProperty("revisions")[0]
            .GetProperty("slots").GetProperty("main").GetProperty("*").GetString();
    }
}
