namespace GameLibrary.Core.Models;

public class GameSource
{
    public Guid Id { get; set; }
    public Guid ProviderId { get; set; }
    public required string Name { get; set; }
    public bool IsPhysical { get; set; }
    public bool IsEmulated { get; set; }
    public required string Config { get; set; }
}
