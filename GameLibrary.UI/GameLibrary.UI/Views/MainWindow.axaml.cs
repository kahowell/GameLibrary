using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace GameLibrary.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        #if DEBUG
        this.AttachDevTools(new KeyGesture(Key.F1));
        #endif
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (e.Key == Key.F11) WindowState = WindowState == WindowState.FullScreen ? WindowState.Normal : WindowState.FullScreen;
    }
}
