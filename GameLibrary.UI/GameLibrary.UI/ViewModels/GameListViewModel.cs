using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using DynamicData;
using DynamicData.Binding;
using GameLibrary.Core;
using GameLibrary.UI.Views;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace GameLibrary.UI.ViewModels;

public partial class GameListViewModel: ViewModelBase
{
    private readonly ILibraryGameRepository _libraryGameRepository;
    private readonly ReadOnlyObservableCollection<GameRowViewModel> _gameRows;
    private readonly SourceCache<GameRowViewModel, Guid> _sourceCache = new (x => x.Id);
    [Reactive] private string? _searchText;
    [Reactive] private bool _isLoading;
    [Reactive] private GameDetailsViewModel? _selectedGame;
    [Reactive] private ViewModelBase? _activeView;
    public ReadOnlyObservableCollection<GameRowViewModel> GameRows => _gameRows;

    public GameListViewModel(ILibraryGameRepository libraryGameRepository, IImageRepository imageRepository)
    {
        _libraryGameRepository = libraryGameRepository;
        var searchFilter = this.WhenValueChanged(s => s.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(200))
            .Select(s => s != null ? s.Trim() : "")
            .DistinctUntilChanged()
            .Select<string, Func<GameRowViewModel, bool>>(text => row => row.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase) );

        Task.Run(async () => await LoadGames(imageRepository));

        _sourceCache.Connect()
            .Filter(searchFilter)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _gameRows)
            .Subscribe();
    }

    public async Task LoadGames(IImageRepository imageRepository, CancellationToken token = default)
    {
        await Dispatcher.UIThread.InvokeAsync(() => IsLoading = true);
        _sourceCache.Edit(change => change.Clear());
        try
        {
            var i = 0;
            foreach (var game in await _libraryGameRepository.GetAllAsync(token))
            {
                _sourceCache.Edit(change => change.AddOrUpdate(new GameRowViewModel(i, game, imageRepository)));
                i++;
            }
        }
        finally
        {
            await Dispatcher.UIThread.InvokeAsync(() => IsLoading = false);
        }
    }
}
