using System;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData.Binding;
using GameLibrary.Core;
using GameLibrary.UI.Controls;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace GameLibrary.UI.ViewModels;

public enum ViewType
{
    ListView,
    GridView
}

public partial class MainViewModel : ViewModelBase
{
    [Reactive] private GameListViewModel _gameListViewModel;
    [Reactive] private ViewModelBase? _activeView;

    public MainViewModel(GameListViewModel gameListViewModel, ILibraryGameRepository libraryGameRepository,
        IImageRepository imageRepository)
    {
        _gameListViewModel = gameListViewModel;
        ActivateListViewCommand = ReactiveCommand.Create(() => { ViewType = ViewType.ListView; });
        ActivateGridViewCommand = ReactiveCommand.Create(() => { ViewType = ViewType.GridView; });
        this.WhenValueChanged(m => m.ViewType)
            .DistinctUntilChanged()
            .Select<ViewType, ViewModelBase>(viewType => viewType switch
            {
                ViewType.ListView => new GameTableViewModel(libraryGameRepository, imageRepository, gameListViewModel),
                ViewType.GridView => new GameGridViewModel(gameListViewModel),
                _ => throw new ArgumentOutOfRangeException(nameof(viewType), viewType, null)
            })
            .BindTo(this, m => m.ActiveView);
    }

    [Reactive] private ViewType _viewType = ViewType.ListView;
    public ReactiveCommand<Unit, Unit> ActivateListViewCommand { get; }
    public ReactiveCommand<Unit, Unit> ActivateGridViewCommand { get; }
}
