using System.CommandLine;
using System.Text.Json;
using GameLibrary.Core;
using GameLibrary.IGDB;
using Option = Google.Protobuf.WellKnownTypes.Option;

namespace GameLibrary.CLI.Commands;

public class ConfigCommand: Command
{
    public ConfigCommand(Configuration config) : base("config", "Manage Game Library configuration")
    {
        AddCommand(new ConfigShowCommand(config));
        AddCommand(new ConfigSetCommand(config));
    }
}

public class ConfigShowCommand : Command
{
    public ConfigShowCommand(Configuration config) : base("show", "Display configuration")
    {
        this.SetHandler(() =>
        {
            Console.WriteLine(config.ToJson());
        });
    }
}

public class ConfigSetCommand : Command
{
    public ConfigSetCommand(Configuration config) : base("set", "Set a configuration option")
    {
        var igdbClientIdOption = new Option<string?>(name: "--igdb-client-id", description: "IGDB Client ID");
        var igdbClientSecretOption = new Option<string?>(name: "--igdb-client-secret", description: "IGDB Client Secret");
        AddOption(igdbClientIdOption);
        AddOption(igdbClientSecretOption);
        // TODO usecache
        // TODO steam options
        this.SetHandler((igdbClientIdValue, igdbClientSecretValue) =>
        {
            if (igdbClientIdValue != null)
            {
                config.Get<IgdbOptions>().ClientId = igdbClientIdValue;
            }

            if (igdbClientSecretValue != null)
            {
                config.Get<IgdbOptions>().ClientSecret = igdbClientSecretValue;
            }
            config.Write();
        }, igdbClientIdOption, igdbClientSecretOption);
    }
}
