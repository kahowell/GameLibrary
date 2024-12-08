using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using DynamicData.Binding;
using GameLibrary.Core;
using GameLibrary.Core.Models;
using GameLibrary.Core.Services;
using GameLibrary.Steam;
using GameLibrary.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
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
    [Reactive] private ViewModelBase? _activeView;
    [Reactive] private bool _isSyncing;
    private readonly Lazy<IScreen> _hostScreen;

    public MainViewModel(Lazy<IScreen> hostScreen, IServiceProvider serviceProvider, Configuration config)
    {
        _hostScreen = hostScreen;
        ActivateListViewCommand = ReactiveCommand.Create(() => { ViewType = ViewType.ListView; });
        ActivateGridViewCommand = ReactiveCommand.Create(() => { ViewType = ViewType.GridView; });
        ShowAboutCommand = ReactiveCommand.Create(() =>
        {
            HostScreen.Router.Navigate.Execute(serviceProvider.GetRequiredService<AboutViewModel>());
        });
        ShowAddLibraryCommand = ReactiveCommand.Create(() =>
        {
            HostScreen.Router.Navigate.Execute(serviceProvider.GetRequiredService<AddLibraryViewModel>());
        });
        var gameList = serviceProvider.GetRequiredService<GameListViewModel>();
        var imageRepository = serviceProvider.GetRequiredService<IImageRepository>();
        SyncLibrariesCommand = ReactiveCommand.Create(() =>
        {
            var syncService = serviceProvider.GetRequiredService<LibrarySyncService>();
            var steamSourceProvider = serviceProvider.GetRequiredService<SteamSourceProvider>();
            IsSyncing = true;
            var steamOptions = config.Get<SteamOptions>() ?? new SteamOptions();
            Task.Run(async () =>
            {
                foreach (var libraryOptions in steamOptions.Libraries)
                {
                    var library = steamSourceProvider.GetLibrary(libraryOptions.SteamId, libraryOptions.DisplayName);
                    var syncTask = syncService.CreateSyncTask(library);
                    syncTask.Subscribe(status =>
                    {
                        if (status.AddedGame != null)
                        {
                            gameList.AddGame(new GameRowViewModel(new LibraryGameSummary(status.AddedGame),
                                imageRepository));
                        }
                    });
                    await syncTask.Run();
                }
                Dispatcher.UIThread.Post(() => IsSyncing = false);
            });
        }, canExecute: this.WhenAnyValue(x => x.IsSyncing).Select(x => !x));
        this.WhenValueChanged(m => m.ViewType)
            .DistinctUntilChanged()
            .Select<ViewType, ViewModelBase>(viewType => viewType switch
            {
                ViewType.ListView => serviceProvider.GetRequiredService<GameTableViewModel>(),
                ViewType.GridView => serviceProvider.GetRequiredService<GameGridViewModel>(),
                _ => throw new ArgumentOutOfRangeException(nameof(viewType), viewType, null)
            })
            .BindTo(this, m => m.ActiveView);
    }

    [Reactive] private ViewType _viewType = ViewType.ListView;
    public ReactiveCommand<Unit, Unit> ActivateListViewCommand { get; }
    public ReactiveCommand<Unit, Unit> ActivateGridViewCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowAboutCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowAddLibraryCommand { get; }
    public ReactiveCommand<Unit, Unit> SyncLibrariesCommand { get; }
    public string? UrlPathSegment => "Main";
    public IScreen HostScreen => _hostScreen.Value;
}
