namespace GameLibrary.Core.Models;

public class LibraryGame
{
    public Guid Id { get; set; }
    public LibraryService? LibraryService { get; set; }
    public required string LibraryId { get; set; }
    public required string ExternalId { get; set; }
    public Release? Release { get; set; }
    public required LibraryGameStatus LibraryGameStatus { get; set; }
}

public class LibraryGameSummary
{
    public Guid Id { get; set; }
    public LibraryService? LibraryService { get; set; }
    public string Name { get; set; }
    public string SortingName { get; set; }
    public Image? CoverImage { get; set; }
    public Region Region { get; set; }
    public DateTimeOffset? ReleaseDate { get; set; }
    public DatePrecision? ReleaseDatePrecision { get; set; }
    public LibraryGameStatus LibraryGameStatus { get; set; }
}

// {All,Library,Genre,Theme,Platform}

public enum LibraryService
{
    Steam,
}

public enum LibraryGameStatus
{
    Available,
    Installing,
    Installed,
    Updating,
    Running,
    Uninstalling,
    TemporarilyUnavailable,
}
