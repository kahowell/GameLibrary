using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GameLibrary.UI.ViewModels;
using ReactiveUI;

namespace GameLibrary.UI.Views;

public partial class FirstRunView : ReactiveUserControl<FirstRunViewModel>
{
    public FirstRunView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
}
