using System;
using DynamicData.Binding;
using GameLibrary.Core;
using GameLibrary.IGDB;
using GameLibrary.Steam;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace GameLibrary.UI.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [Reactive] private string? _igdbClientId;
    [Reactive] private string? _igdbClientSecret;
    [Reactive] private bool _isValid;
    [Reactive] private Configuration _config;

    public SettingsViewModel(Configuration config)
    {
        _config = config;
        LoadValues();
        this.WhenAnyValue(x => x.IgdbClientId, x => x.IgdbClientSecret,
            (clientId, clientSecret) => clientId != null && clientSecret != null).BindTo(this, x => x.IsValid);
    }

    public void Save()
    {
        if (!IsValid) throw new ArgumentException("Invalid configuration");
        var igdbOptions = Config.Get<IgdbOptions>();
        if (igdbOptions == null)
        {
            Config.Set(nameof(IgdbOptions), new IgdbOptions()
            {
                ClientId = _igdbClientId!,
                ClientSecret = _igdbClientSecret!,
            });
        }
        else
        {
            igdbOptions.ClientId = IgdbClientId!;
            igdbOptions.ClientSecret = IgdbClientSecret!;
        }

        Config.Write();
    }

    public void LoadValues()
    {
        var igdbOptions = Config.Get<IgdbOptions>();
        IgdbClientId = igdbOptions?.ClientId;
        IgdbClientSecret = igdbOptions?.ClientSecret;
    }
}
