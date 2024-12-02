using Avalonia.Controls;
using Avalonia.ReactiveUI;
using GameLibrary.UI.ViewModels;

namespace GameLibrary.UI.Views;

public partial class MainView : ReactiveUserControl<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();
    }
}
