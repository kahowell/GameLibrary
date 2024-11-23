namespace GameLibrary.Steam;

public class SteamOptions
{
    public required string ApiKey { get; set; }
    public bool UseCache { get; set; } = false;
}
