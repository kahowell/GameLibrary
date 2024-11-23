namespace GameLibrary.Core.Models;

public class Platform
{
    public Guid Id { get; set; }
    public ulong? IgdbId { get; set; }
    public required string Name { get; set; }
    public Image? Logo { get; set; }
}
