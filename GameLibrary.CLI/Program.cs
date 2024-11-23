using System.CommandLine;
using GameLibrary.CLI.Commands;
using GameLibrary.Core;
using GameLibrary.IGDB;
using GameLibrary.PCGamingWiki;
using GameLibrary.SQLite;
using GameLibrary.Steam;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var appBuilder = Host.CreateApplicationBuilder();
var services = appBuilder.Services;

services.AddGameLibraryCoreServices();
services.AddIgdbServices();
services.AddSqliteServices();
services.AddSteamServices();
services.AddPcGamingWikiService();
services.AddSingleton<Command, InitCommand>();
services.AddSingleton<Command, ConfigCommand>();
services.AddSingleton<Command, SyncCommand>();
services.AddSingleton<Command, PcGamingWikiCommand>();

var serviceProvider = services.BuildServiceProvider();
var rootCommand = new RootCommand("GameLibrary CLI");
var subCommands = serviceProvider.GetServices<Command>();
var dbContext = serviceProvider.GetRequiredService<LibraryContext>();
await dbContext.Database.MigrateAsync();

foreach (var subCommand in subCommands)
{
    rootCommand.AddCommand(subCommand);
}

await rootCommand.InvokeAsync(args);
