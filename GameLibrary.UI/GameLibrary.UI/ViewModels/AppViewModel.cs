using System;
using GameLibrary.Core;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Splat;

namespace GameLibrary.UI.ViewModels;

public partial class AppViewModel : ViewModelBase, IScreen
{
    [Reactive] private RoutingState _router = new();

    public AppViewModel(Configuration config, FirstRunViewModel firstRunViewModel, MainViewModel mainViewModel)
    {
        if (config.Exists())
        {
            Router.Navigate.Execute(mainViewModel);
        }
        else
        {
            firstRunViewModel.NextViewModel = mainViewModel;
            Router.Navigate.Execute(firstRunViewModel);
        }
    }
}
