using System;
using System.IO;
using System.Reactive;
using System.Text;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GameLibrary.UI.Assets;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace GameLibrary.UI.ViewModels;

public partial class AboutViewModel : ViewModelBase, IRoutableViewModel
{
    [Reactive] private string _licenseHeader = string.Format(Resources.AboutLicenseHeader, Resources.AppName);
    [Reactive] private string _creditsText = LoadTextFile("CREDITS.txt");
    [Reactive] private string _licenseText = LoadTextFile("LICENSE.txt");
    [Reactive] private string _fontAwesomeLicenseText = LoadTextFile(@"Fonts\FontAwesome-LICENSE.txt");
    private readonly Lazy<IScreen> _hostScreen;

    public AboutViewModel(Lazy<IScreen> hostScreen)
    {
        _hostScreen = hostScreen;
        BackCommand = ReactiveCommand.Create(() =>
        {
            HostScreen.Router.NavigateBack.Execute();
        });
    }

    public AboutViewModel(): this(null!)
    {
    }

    private static string LoadTextFile(string filename)
    {
        using var buffer = new MemoryStream();
        using var stream = AssetLoader.Open(new Uri($"avares://GameLibrary.UI/Assets/{filename}"));
        stream.CopyTo(buffer);
        return Encoding.UTF8.GetString(buffer.ToArray());
    }

    public string? UrlPathSegment => "about";
    public IScreen HostScreen => _hostScreen.Value;
    public ReactiveCommand<Unit, Unit> BackCommand { get; }
}
