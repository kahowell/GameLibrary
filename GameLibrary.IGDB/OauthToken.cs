namespace GameLibrary.IGDB;

public class OauthToken
{
    private readonly double _expiresIn;
    public required string AccessToken { get; init; }

    public required double ExpiresIn
    {
        get => _expiresIn;
        init
        {
            _expiresIn = value;
            ExpiresAt = DateTime.UtcNow.AddSeconds(_expiresIn);
        }
    }

    public DateTimeOffset ExpiresAt { get; init; }

    public bool IsExpired() { return ExpiresAt < DateTimeOffset.Now; }
}
