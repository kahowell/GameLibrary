﻿using System;
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

public partial class MainViewModel : ViewModelBase, IRoutableViewModel
{
    [Reactive] private GameListViewModel _gameListViewModel;
    [Reactive] private ViewModelBase? _activeView;
    private readonly Lazy<IScreen> _hostScreen;

    public MainViewModel(Lazy<IScreen> hostScreen, GameListViewModel gameListViewModel, ILibraryGameRepository libraryGameRepository,
        IImageRepository imageRepository)
    {
        _hostScreen = hostScreen;
        _gameListViewModel = gameListViewModel;
        ActivateListViewCommand = ReactiveCommand.Create(() => { ViewType = ViewType.ListView; });
        ActivateGridViewCommand = ReactiveCommand.Create(() => { ViewType = ViewType.GridView; });
        ShowAboutCommand = ReactiveCommand.Create(() => { HostScreen.Router.Navigate.Execute(new AboutViewModel(HostScreen)); });
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
    public ReactiveCommand<Unit, Unit> ShowAboutCommand { get; }
    public string? UrlPathSegment => "Main";
    public IScreen HostScreen => _hostScreen.Value;
}
