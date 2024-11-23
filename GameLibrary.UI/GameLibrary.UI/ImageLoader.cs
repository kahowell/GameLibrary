using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using GameLibrary.Core;
using GameLibrary.Core.Models;

namespace GameLibrary.UI;

public static class ImageLoader
{
    private static readonly SingleThreadTaskScheduler SingleThreadTaskScheduler = new("ImageLoader thread");
    private static readonly TaskFactory TaskFactory = new(SingleThreadTaskScheduler);
    public static async Task<Bitmap> LoadImage(IImageRepository imageRepository, Image image, CancellationToken cancellationToken = default)
    {
        return await TaskFactory.StartNew(async () =>
        {
            await using var stream = await imageRepository.DownloadAsync(image);
            return new Bitmap(stream);
        }, cancellationToken).Unwrap();
    }

    public static Task<Bitmap> LoadImageToWidth(IImageRepository imageRepository, Image image, int width, CancellationToken cancellationToken = default)
    {
        return TaskFactory.StartNew(async () =>
        {
            await using var stream = await imageRepository.DownloadAsync(image);
            return Bitmap.DecodeToWidth(stream, width);
        }, cancellationToken).Unwrap();
    }
}

public class SingleThreadTaskScheduler : TaskScheduler
{
    private readonly BlockingCollection<Task> _taskQueue = new();

    public SingleThreadTaskScheduler(string name)
    {
        var thread = new Thread(Start)
        {
            Name = name,
            IsBackground = true
        };
        thread.Start();
    }

    private void Start()
    {
        try
        {
            while (true)
            {
                if (_taskQueue.TryTake(out var task))
                {
                    TryExecuteTask(task);
                }
            }
        }
        catch (ObjectDisposedException)
        {
            /* intentionally empty */
        }
    }

    protected override IEnumerable<Task> GetScheduledTasks()
    {
        return _taskQueue.AsEnumerable();
    }

    protected override void QueueTask(Task task)
    {
        _taskQueue.Add(task);
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        return false;
    }
}
