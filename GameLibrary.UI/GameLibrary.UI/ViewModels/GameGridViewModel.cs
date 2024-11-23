using System.Linq;
using System.Reactive.Linq;
using Avalonia.Controls;
using DynamicData.Binding;
using GameLibrary.Core.Mocks;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace GameLibrary.UI.ViewModels;

public partial class GameGridViewModel(GameListViewModel gameListViewModel) : ViewModelBase
{
    [Reactive] private GameListViewModel _gameListViewModel = gameListViewModel;
    [Reactive] private int _gamesPerRow = 4;

    public GameGridViewModel() : this(new GameListViewModel(new MockGameRepository(), new MockImageRepository()))
    {
    }
}
