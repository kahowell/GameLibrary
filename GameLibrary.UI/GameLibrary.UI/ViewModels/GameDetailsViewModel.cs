using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using GameLibrary.Core;
using GameLibrary.Core.Models;

namespace GameLibrary.UI.ViewModels;

public class GameDetailsViewModel(LibraryGame libraryGame, IImageRepository imageRepository) : ViewModelBase
{
    public Guid Id => libraryGame.Id;
    public string? Name => libraryGame.Release?.Game?.Name;
    public DateTimeOffset? ReleaseDate => libraryGame.Release?.ReleaseDate;
    public IList<Company> Developers => libraryGame.Release?.Game?.Developers ?? [];
    public IList<Company> Publishers => libraryGame.Release?.Game?.Publishers ?? [];
    public string? Description => libraryGame.Release?.Game?.Description;
    public Task<Bitmap?> BackgroundImageAsync => GetBackgroundImageAsync();

    private async Task<Bitmap?> GetBackgroundImageAsync()
    {
        if (libraryGame.Release?.Game?.BackgroundImage == null) return null;
        return await ImageLoader.LoadImage(imageRepository, libraryGame.Release.Game.BackgroundImage);
    }
}
