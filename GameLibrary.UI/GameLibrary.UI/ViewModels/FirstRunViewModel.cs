using System;
using System.Reactive;
using Avalonia.Platform;
using DynamicData.Binding;
using GameLibrary.UI.Assets;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace GameLibrary.UI.ViewModels;

public partial class FirstRunViewModel : ViewModelBase, IRoutableViewModel
{
    [Reactive] private SettingsViewModel _settings;
    [Reactive] private string _explanation = string.Format(Resources.FirstRunExplanation, Resources.AppName);
    private readonly Lazy<IScreen> _hostScreen;
    [Reactive] private IRoutableViewModel? _nextViewModel;
    public string UrlPathSegment => nameof(FirstRunViewModel);
    public IScreen HostScreen => _hostScreen.Value;
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    public FirstRunViewModel(Lazy<IScreen> hostScreen, SettingsViewModel settings)
    {
        Settings = settings;
        _hostScreen = hostScreen;
        SaveCommand = ReactiveCommand.Create(SaveSettings, canExecute: Settings.WhenValueChanged(s => s.IsValid));
    }

    private void SaveSettings()
    {
        Settings.Save();
        if (NextViewModel != null) HostScreen.Router.Navigate.Execute(NextViewModel);
    }
}
