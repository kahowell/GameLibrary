using GameLibrary.Core.Models;

namespace GameLibrary.Core.Mocks;

public class MockImageRepository: IImageRepository
{
    private readonly IList<Image> _images = [
        new() { Url = "test.jpg", Height = 100, Width = 100, ImageType = ImageType.Cover}
    ];

    public Task<Stream> DownloadAsync(Image image) => Task.FromResult(Stream.Null);

    public Task<int> Count() => Task.FromResult(_images.Count);

    public Task DeleteAll() => Task.CompletedTask;

    public Task<Image?> FindExistingAsync(Image image) => Task.FromResult<Image?>(null);

    public Task<Image> UploadAsync(Image image, Stream stream) => Task.FromResult(image);
}
