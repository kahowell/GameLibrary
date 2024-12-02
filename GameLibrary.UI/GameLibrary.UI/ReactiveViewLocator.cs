using System;
using GameLibrary.UI.ViewModels;
using GameLibrary.UI.Views;
using ReactiveUI;

namespace GameLibrary.UI;

public class ReactiveViewLocator : IViewLocator
{
    public IViewFor? ResolveView<T>(T? viewModel, string? contract = null) => viewModel switch
    {
        FirstRunViewModel => new FirstRunView(),
        MainViewModel => new MainView(),
        AboutViewModel => new AboutView(),
        _ => throw new ArgumentOutOfRangeException(nameof(viewModel), viewModel, null)
    };
}
