using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GameLibrary.UI.ViewModels;
using ReactiveUI;

namespace GameLibrary.UI.Views;

public partial class AddLibraryView : ReactiveUserControl<AddLibraryViewModel>
{
    public AddLibraryView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
}
