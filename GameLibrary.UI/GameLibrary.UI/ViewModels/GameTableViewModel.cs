using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using DynamicData;
using GameLibrary.Core;
using GameLibrary.Core.Mocks;
using GameLibrary.Core.Models;
using GameLibrary.UI.Converters;
using ReactiveUI.SourceGenerators;
using Image = Avalonia.Controls.Image;

namespace GameLibrary.UI.ViewModels;

public partial class GameTableViewModel : ViewModelBase
{
    [Reactive] private FlatTreeDataGridSource<GameRowViewModel> _gameRowsSource;
    [Reactive] private GameListViewModel _gameListViewModel;

    public GameTableViewModel(ILibraryGameRepository libraryGameRepository, IImageRepository imageRepository,
        GameListViewModel gameListViewModel)
    {
        _gameListViewModel = gameListViewModel;
        GameRowsSource = new FlatTreeDataGridSource<GameRowViewModel>(gameListViewModel.GameRows)
        {
            Columns =
            {
                new TemplateColumn<GameRowViewModel>("Cover", new FuncDataTemplate<GameRowViewModel>((_, _) =>
                    new Image
                    {
                        [!Image.SourceProperty] = new Binding($"{nameof(GameRowViewModel.CoverImageAsync)}^"),
                        Width = 150,
                        Height = 200
                    }
                )),
                new TemplateColumn<GameRowViewModel>("Name", new FuncDataTemplate<GameRowViewModel>((_, _) =>
                        new TextBlock
                        {
                            [!TextBlock.TextProperty] = new Binding($"{nameof(GameRowViewModel.Name)}"),
                            TextWrapping = TextWrapping.WrapWithOverflow,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = Thickness.Parse("15")
                        }),
                    width: GridLength.Parse("4*"),
                    options: new TemplateColumnOptions<GameRowViewModel>
                    {
                        CompareAscending =
                            (a, b) => string.Compare(a?.Name, b?.Name, StringComparison.InvariantCulture),
                        CompareDescending = (a, b) =>
                            string.Compare(b?.Name, a?.Name, StringComparison.InvariantCulture),
                        CanUserSortColumn = true
                    }
                ),
                new TemplateColumn<GameRowViewModel>("Release Date", new FuncDataTemplate<GameRowViewModel>((_, _) =>
                    new TextBlock
                    {
                        [!TextBlock.TextProperty] = new Binding($"{nameof(GameRowViewModel.ReleaseDate)}")
                        {
                            Converter = new DateConverter()
                        },
                        TextWrapping = TextWrapping.NoWrap,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = Thickness.Parse("15"),
                    }), options: new TemplateColumnOptions<GameRowViewModel>
                {
                    CompareAscending = (a, b) => Nullable.Compare(a?.ReleaseDate, b?.ReleaseDate),
                    CompareDescending = (a, b) => Nullable.Compare(b?.ReleaseDate, a?.ReleaseDate),
                    CanUserSortColumn = true
                }),
                new TemplateColumn<GameRowViewModel>("Library", new FuncDataTemplate<GameRowViewModel>((_, _) =>
                    new TextBlock
                    {
                        [!TextBlock.TextProperty] = new Binding($"{nameof(GameRowViewModel.Service)}"),
                        TextWrapping = TextWrapping.NoWrap,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = Thickness.Parse("15")
                    }), width: GridLength.Parse("*")),
                new TemplateColumn<GameRowViewModel>("Status", new FuncDataTemplate<GameRowViewModel>((_, _) =>
                    new TextBlock
                    {
                        [!TextBlock.TextProperty] = new Binding($"{nameof(GameRowViewModel.Status)}"),
                        TextWrapping = TextWrapping.NoWrap,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = Thickness.Parse("15")
                    }), width: GridLength.Parse("*"), options: new TemplateColumnOptions<GameRowViewModel>
                {
                    CompareAscending = (a, b) => Nullable.Compare(a?.Status, b?.Status),
                    CompareDescending = (a, b) => Nullable.Compare(b?.Status, a?.Status),
                    CanUserSortColumn = true
                })
            }
        };
        if (gameListViewModel.SelectedGame != null)
        {
            var index = gameListViewModel.GameRows.Select(g => g.Id).IndexOf(gameListViewModel.SelectedGame.Id);
            GameRowsSource.RowSelection?.Select(GameRowsSource.Rows.ModelIndexToRowIndex(index));
        }
        GameRowsSource.RowSelection!.SelectionChanged += async (_, args) =>
        {
            gameListViewModel.SelectedGame = new GameDetailsViewModel((await args.SelectedItems
                .OfType<GameRowViewModel>()
                .Select(async s => await libraryGameRepository.GetLibraryGameByIdAsync(s.Id))
                .FirstOrDefault()!)!, imageRepository);
        };
    }

    public GameTableViewModel() : this(new MockGameRepository(), new MockImageRepository(),
        new GameListViewModel(new MockGameRepository(), new MockImageRepository()))
    {
        /* intentionally empty */
    }
}

public class GameRowViewModel(int index, LibraryGameSummary libraryGame, IImageRepository imageRepository) : ViewModelBase
{
    public int Index { get; } = index;
    public Guid Id => libraryGame.Id;
    public LibraryService? Service => libraryGame.LibraryService;
    public LibraryGameStatus Status => libraryGame.LibraryGameStatus;
    public string Name => libraryGame.Name;
    public string EffectiveSortingName => libraryGame.SortingName;
    public DateTimeOffset? ReleaseDate => libraryGame.ReleaseDate;

    public Task<Bitmap?> CoverImageAsync => GetCoverImageAsync();

    private async Task<Bitmap?> GetCoverImageAsync()
    {
        if (libraryGame.CoverImage == null) return null;
        return await ImageLoader.LoadImageToWidth(imageRepository, libraryGame.CoverImage, 300);
    }
}
