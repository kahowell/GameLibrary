using GameLibrary.Core;
using GameLibrary.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace GameLibrary.SQLite;

public class ImageRepository(IDbContextFactory<LibraryContext> libraryContextFactory): IImageRepository
{
    public async IAsyncEnumerable<Image> GetAll()
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        await foreach (var image in libraryContext.Images.AsAsyncEnumerable())
        {
            yield return image;
        }
    }

    public async Task<Image?> FindExistingAsync(Image image)
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        return await libraryContext.Images.Where(i =>
            i.Id == image.Id || (image.IgdbImageId != null && image.IgdbImageId == i.IgdbImageId)).FirstOrDefaultAsync();
    }

    public async Task<Image> UploadAsync(Image image, Stream stream)
    {
        Console.WriteLine($"Uploading {image.Id}");
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        using var output = new MemoryStream();
        await stream.CopyToAsync(output);
        var imageData = new ImageData
        {
            Image = image,
            Data = output.ToArray()
        };
        libraryContext.ImageData.Update(imageData);
        await libraryContext.SaveChangesAsync();
        return image;
    }

    public async Task<Stream> DownloadAsync(Image image)
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        var imageData = await libraryContext.ImageData.Where(x => x.Image.Id == image.Id).FirstAsync();
        if (imageData == null) throw new ArgumentException($"Image with id {image.Id} not found");
        return new MemoryStream(imageData.Data.ToArray());
    }

    public async Task<int> Count()
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        return await libraryContext.Images.CountAsync();
    }

    public async Task DeleteAll()
    {
        await using var libraryContext = await libraryContextFactory.CreateDbContextAsync();
        await libraryContext.Images.ExecuteDeleteAsync();
        await libraryContext.SaveChangesAsync();
    }
}
