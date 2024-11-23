namespace GameLibrary.Core.Models;

public class Game
{
    public Guid Id { get; set; }
    public ulong? IgdbId { get; set; }
    public string? Name { get; set; }
    public GameType? Type { get; set; }
    public string? Description { get; set; }
    public string? SortingName { get; set; }
    public Image? BackgroundImage { get; set; }
    public Image? CoverImage { get; set; }
    public IList<Release> Releases { get; set; } = [];
    public DateTimeOffset? ReleaseDate { get; set; }
    public DatePrecision? ReleaseDatePrecision { get; set; }
    public IList<Company> Developers { get; set; } = [];
    public IList<Company> Publishers { get; set; } = [];
    public IList<Theme> Themes { get; set; } = [];
    public IList<Genre> Genres { get; set; } = [];
    public IList<Keyword> Keywords { get; set; } = [];
    public IList<MultiplayerMode> MultiplayerModes { get; set; } = [];

    public string? GetEffectiveSortingName()
    {
        return SortingName ?? Name;
    }
}

public class Release
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public Game? Game { get; set; }
    public ulong? IgdbId { get; set; }
    public DateTimeOffset? ReleaseDate { get; set; }
    public DatePrecision? ReleaseDatePrecision { get; set; }
    public Region Region { get; set; }
    public required Platform Platform { get; set; }
}

public enum Region
{
    Unknown = 0,
    Europe = 1,
    NorthAmerica = 2,
    Australia = 3,
    NewZealand = 4,
    Japan = 5,
    China = 6,
    Asia = 7,
    Worldwide = 8,
    Korea = 9,
    Brazil = 10,
}

public enum GameType
{
    Game,
    Dlc,
    Mod,
}

public class Theme
{
    public Guid Id { get; set; }
    public ulong? IgdbId { get; set; }
    public string Name { get; set; }
}

public class Genre
{
    public Guid Id { get; set; }
    public ulong? IgdbId { get; set; }
    public string Name { get; set; }
}

public class Keyword
{
    public Guid Id { get; set; }
    public ulong? IgdbId { get; set; }
    public string Name { get; set; }
}

public class MultiplayerMode
{
    public Guid Id { get; set; }
    public ulong? IgdbId { get; set; }
    public string Name { get; set; }
}
