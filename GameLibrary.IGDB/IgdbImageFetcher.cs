using System.Runtime.InteropServices;
using GameLibrary.Core;
using GameLibrary.Core.Models;
using Microsoft.VisualBasic.FileIO;

namespace GameLibrary.IGDB;

public class IgdbImageFetcher(HttpClient client, Configuration configuration)
{
    private readonly IgdbOptions _options = configuration.Get<IgdbOptions>();
    private readonly string _cacheDir = Configuration.EnsureCacheDirectory(Path.Join("GameLibrary.IGDB", "images"));

    public async Task<Stream> FetchImageAsync(Image image)
    {
        string? cache = null;
        if (image.IgdbImageId == null) throw new NotSupportedException("Can't fetch images for another source");
        var igdbId = image.IgdbImageId;
        if (_options.UseCache)
        {
            cache = Path.Join(_cacheDir, igdbId);
            if (Path.Exists(cache))
            {
                Console.WriteLine("Reading query results from cache");
                return File.OpenRead(cache);
            }
        }
        var extension = image.ImageType switch
        {
            ImageType.Cover => "jpg",
            ImageType.Logo => "png",
            ImageType.Screenshot => "jpg"
        };
        var size = image.ImageType switch
        {
            ImageType.Cover => "cover_big",
            ImageType.Logo => "logo_med",
            ImageType.Screenshot => "1080p"
        };
        var sizeSuffix = image.ImageType switch
        {
            ImageType.Cover => "_2x",
            ImageType.Logo => "",
            ImageType.Screenshot => "_2x"
        };
        var response = await client.GetAsync($"igdb/image/upload/t_{size}{sizeSuffix}/{igdbId}.{extension}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsByteArrayAsync();
        if (cache != null)
        {
            await using var stream = new FileStream(cache, FileMode.Create, FileAccess.Write, FileShare.Read);
            await stream.WriteAsync(content);
        }

        return new MemoryStream(content);
    }
}
