using System.Diagnostics.CodeAnalysis;
using GameLibrary.Core;

namespace GameLibrary.IGDB;

public class IgdbOptions
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public OauthToken? Token { get; set; }
    public bool UseCache { get; set; } = false;
}
