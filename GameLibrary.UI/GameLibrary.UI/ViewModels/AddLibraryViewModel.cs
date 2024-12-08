using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using DynamicData.Binding;
using GameFinder.Common;
using GameLibrary.Core;
using GameLibrary.Core.Mocks;
using GameLibrary.Core.Models;
using GameLibrary.Steam;
using GameLibrary.UI.Assets;
using GameLibrary.UI.Controls;
using GameLibrary.UI.Converters;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace GameLibrary.UI.ViewModels;

public partial class AddLibraryViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly Lazy<IScreen> _hostScreen;
    private readonly Configuration _config;
    [Reactive] private FlatTreeDataGridSource<SteamLibraryViewModel> _discoveredSteamLibrarySource;

    public AddLibraryViewModel(Lazy<IScreen> hostScreen, SteamSourceProvider steamLibrarySource, Configuration config)
    {
        _hostScreen = hostScreen;
        _config = config;
        BackCommand = ReactiveCommand.Create(() => { HostScreen.Router.NavigateBack.Execute(); });
        var availableSteamLibraries = steamLibrarySource.GetAvailableLibrariesAsync().Result
            .Select(x =>
            {
                var username = x.DisplayName;
                var apiKey = GetApiKey(x);
                var exists = apiKey is not null;
                return new SteamLibraryViewModel(x.LibraryId, username, apiKey, exists);
            });
        DiscoveredSteamLibrarySource = new FlatTreeDataGridSource<SteamLibraryViewModel>(availableSteamLibraries)
        {
            Columns =
            {
                new TextColumn<SteamLibraryViewModel, string>(Resources.LabelSteamUser, x => x.Username),
                new TemplateColumn<SteamLibraryViewModel>(Resources.LabelApiKey, new FuncDataTemplate<SteamLibraryViewModel>(SteamApiKeyTextField)),
                new TemplateColumn<SteamLibraryViewModel>(string.Empty, new FuncDataTemplate<SteamLibraryViewModel>(AddRemoveSteamLibraryButton))
            }
        };
    }

    public AddLibraryViewModel() : this(App.ServiceProvider!.GetRequiredService<Lazy<IScreen>>(),
        App.ServiceProvider!.GetRequiredService<SteamSourceProvider>(),
        App.ServiceProvider!.GetRequiredService<Configuration>())
    {
    }

    private static TextBox SteamApiKeyTextField(SteamLibraryViewModel gameLibrary, INameScope scope)
    {
        return new TextBox
        {
            [!TextBox.TextProperty] = new Binding(nameof(SteamLibraryViewModel.ApiKey)),
            Width = 150
        };
    }

    private Panel AddRemoveSteamLibraryButton(SteamLibraryViewModel gameLibrary, INameScope scope)
    {
        var panel = new Panel();
        panel.Children.Add(new ButtonWithIcon
        {
            Icon = "minus",
            Text = Resources.ButtonLabelRemoveSteamLibrary,
            Command = ReactiveCommand.Create(() =>
            {
                var steamOptions = _config.Get<SteamOptions>();
                var existing = steamOptions!.Libraries.First(x => x.SteamId == gameLibrary.SteamId);
                steamOptions.Libraries.Remove(existing);
                _config.Write();
                gameLibrary.IsPresent = false;
            }),
            [!Visual.IsVisibleProperty] = gameLibrary.WhenAnyValue(x => x.IsPresent).ToBinding()
        });
        var isValidObservable = gameLibrary.WhenAnyValue(
            x => x.ApiKey,
            x => !string.IsNullOrWhiteSpace(x));
        panel.Children.Add(new ButtonWithIcon
        {
            Icon = "plus",
            Text = Resources.ButtonLabelAddSteamLibrary,
            Command = ReactiveCommand.Create(() =>
            {
                var steamOptions = _config.Get<SteamOptions>();
                if (steamOptions is null)
                {
                    steamOptions = new SteamOptions();
                    _config.Set(nameof(SteamOptions), steamOptions);
                }

                var steamLibraryOptions = new SteamLibraryOptions
                {
                    ApiKey = gameLibrary.ApiKey,
                    SteamId = gameLibrary.SteamId,
                    DisplayName = gameLibrary.Username
                };
                steamOptions.Libraries.Add(steamLibraryOptions);
                _config.Write();
                gameLibrary.IsPresent = true;
            }, isValidObservable),
            [!Visual.IsVisibleProperty] = gameLibrary
                .WhenAnyValue(x => x.IsPresent)
                .Select(x => !x)
                .ToBinding()
        });
        return panel;
    }

    private string? GetApiKey(IGameLibrary gameLibrary)
    {
        var steamOptions = _config.Get<SteamOptions>() ?? new SteamOptions();
        return steamOptions.Libraries
            .Where(x => x.SteamId == gameLibrary.LibraryId)
            .Select(x => x.ApiKey)
            .FirstOrDefault();
    }

    public string UrlPathSegment => "add_library";
    public IScreen HostScreen => _hostScreen.Value;
    public ReactiveCommand<Unit, Unit> BackCommand { get; }
}

public partial class SteamLibraryViewModel(string steamId, string username, string? apiKey, bool isPresent) : ViewModelBase
{
    [Reactive] private string _steamId = steamId;
    [Reactive] private string _username = username;
    [Reactive] private string? _apiKey = apiKey;
    [Reactive] private bool _isPresent = isPresent;
}
