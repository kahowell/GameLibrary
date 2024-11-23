using GameLibrary.Core.Models;

namespace GameLibrary.Core;

public interface IImageRepository
{
    Task<Stream> DownloadAsync(Image image);
    Task<int> Count();
    Task DeleteAll();
    Task<Image?> FindExistingAsync(Image image);
    Task<Image> UploadAsync(Image image, Stream stream);
}
