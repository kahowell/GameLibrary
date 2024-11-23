namespace GameLibrary.Core.Models;

public class Company
{
    public Guid Id { get; set; }
    public ulong? IgdbId { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public Image? Logo { get; set; }
}
