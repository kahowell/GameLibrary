namespace GameLibrary.Core.Models;

public class Image
{
    public Guid Id { get; set; }
    public string? IgdbImageId { get; set; }
    public required ImageType ImageType { get; set; }
    public string? Url { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public enum ImageType
{
    Cover,
    Logo,
    Screenshot,
}
