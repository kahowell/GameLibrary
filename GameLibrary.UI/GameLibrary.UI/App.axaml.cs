using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using GameLibrary.Core;
using GameLibrary.IGDB;
using GameLibrary.SQLite;
using GameLibrary.Steam;
using GameLibrary.UI.ViewModels;
using GameLibrary.UI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace GameLibrary.UI;

public partial class App : Application
{
    public static IServiceProvider? ServiceProvider { get; set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Assets.Resources.Culture = CultureInfo.CurrentCulture;
        var collection = new ServiceCollection();
        collection.AddSqliteServices();
        collection.AddIgdbServices();
        collection.AddSteamServices();
        collection.AddGameLibraryCoreServices();
        collection.AddSingleton<IScreen, AppViewModel>();
        collection.AddSingleton<Lazy<IScreen>>(s => new Lazy<IScreen>(s.GetRequiredService<IScreen>));
        collection.AddSingleton<AppViewModel>();
        collection.AddSingleton<AboutViewModel>();
        collection.AddTransient<AddLibraryViewModel>();
        collection.AddTransient<SettingsViewModel>();
        collection.AddSingleton<GameListViewModel>();
        collection.AddSingleton<GameTableViewModel>();
        collection.AddSingleton<GameGridViewModel>();
        collection.AddSingleton<FirstRunViewModel>();
        collection.AddSingleton<MainViewModel>();
        var services = collection.BuildServiceProvider();
        ServiceProvider = services;
        var dbContext = services.GetRequiredService<LibraryContext>();
        dbContext.Database.Migrate();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow = new MainWindow
            {
                DataContext = services.GetRequiredService<IScreen>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = services.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
