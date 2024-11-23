using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using GameLibrary.Core;
using GameLibrary.Core.Models;
using GameLibrary.IGDB.Proto;
using Google.Protobuf;
using Company = GameLibrary.Core.Models.Company;
using Game = GameLibrary.Core.Models.Game;
using Platform = GameLibrary.Core.Models.Platform;
using Region = GameLibrary.Core.Models.Region;

namespace GameLibrary.IGDB;

public class IgdbDataSource(HttpClient client, Configuration configuration, IgdbImageFetcher imageFetcher) : IDataSource
{
    private const int DefaultPageSize = 100;
    private readonly IgdbOptions _options = configuration.Get<IgdbOptions>();
    private readonly string _cacheDir = Configuration.EnsureCacheDirectory(Path.Join("GameLibrary.IGDB", "queries"));

    private static readonly IEnumerable<string> GameFields = [
        "name",
        "summary",
        "category",
        "cover.image_id",
        "cover.width",
        "cover.height",
        "first_release_date",
        "release_dates.date",
        "release_dates.platform.name",
        "release_dates.platform.platform_logo.image_id",
        "release_dates.platform.platform_logo.width",
        "release_dates.platform.platform_logo.height",
        "involved_companies.company.id",
        "involved_companies.company.name",
        "involved_companies.company.start_date",
        "involved_companies.company.logo.image_id",
        "involved_companies.company.logo.width",
        "involved_companies.company.logo.height",
        "involved_companies.developer",
        "involved_companies.publisher",
        "artworks.image_id",
        "artworks.width",
        "artworks.height",
        "screenshots.image_id",
        "screenshots.width",
        "screenshots.height"
    ];

    public class QueryCriteria
    {
        public required string Endpoint { get; init; }
        public string? Where { get; init; }
        public IEnumerable<string> Fields { get; init; } = ["*"];
        public int Offset { get; init; }
        public int Limit { get; init; } = DefaultPageSize;

        public string Sha256()
        {
            var buffer = new MemoryStream();
            buffer.Write(Encoding.UTF8.GetBytes(Endpoint));
            buffer.Write(Encoding.UTF8.GetBytes(Where ?? string.Empty));
            foreach (var field in Fields)
            {
                buffer.Write(Encoding.UTF8.GetBytes(field));
            }
            buffer.Write(Encoding.UTF8.GetBytes(Offset.ToString()));
            buffer.Write(Encoding.UTF8.GetBytes(Limit.ToString()));
            buffer.Seek(0, SeekOrigin.Begin);
            var sha256 = SHA256.Create();
            sha256.Initialize();
            var hash = sha256.ComputeHash(buffer);
            return Convert.ToHexString(hash).ToLower();
        }
    }

    public async Task<T?> Query<T>(QueryCriteria queryCriteria, MessageParser<T> parser) where T : IMessage<T>
    {
        string? cache = null;
        if (_options.UseCache)
        {
            cache = Path.Join(_cacheDir, queryCriteria.Sha256());
            if (Path.Exists(cache))
            {
                Console.WriteLine("Reading query results from cache");
                var contents = await File.ReadAllBytesAsync(cache);
                return parser.ParseFrom(contents);
            }
        }
        List<string> components = ["fields " + string.Join(",", queryCriteria.Fields)];
        if (queryCriteria.Where != null) components.Add(queryCriteria.Where);
        components.Add($"offset {queryCriteria.Offset}");
        components.Add($"limit {queryCriteria.Limit}");
        var fullQuery = string.Join(";", components);
        var response = await client.PostAsync(queryCriteria.Endpoint,
            new ByteArrayContent(
                Encoding.UTF8.GetBytes($"{fullQuery};".ToArray())));
        var content = await response.Content.ReadAsByteArrayAsync();
        if (cache != null)
        {
            await using var output = new FileStream(cache, FileMode.Create, FileAccess.Write, FileShare.None);
            await output.WriteAsync(content);
        }

        try
        {
            return parser.ParseFrom(content);
        }
        catch (InvalidProtocolBufferException)
        {
            throw new ArgumentException(Encoding.UTF8.GetString(content));
        }
    }

    public Task<IEnumerable<Platform>> GetAllPlatforms()
    {
        return Task.FromResult(GetAllPlatformsAsync().ToBlockingEnumerable());
    }
    public async IAsyncEnumerable<Platform> GetAllPlatformsAsync()
    {
        var offset = 0;
        while (true)
        {
            var result = await Query(new QueryCriteria()
            {
                Endpoint = "v4/platforms.pb",
                Offset = offset,
            }, PlatformResult.Parser);

            foreach (var platform in result.Platforms)
            {
                yield return new Platform
                {
                    Name = platform.Name,
                    IgdbId = platform.Id
                };
            }

            if (result.Platforms.Count < DefaultPageSize)
            {
                break;
            }

            offset += DefaultPageSize;
        }
    }

    private async Task<Proto.Game?> GetSteamGame(string steamAppId)
    {
        var externalGameResult = await Query(new QueryCriteria
        {
            Endpoint = "v4/external_games.pb",
            Fields = GameFields.Select(field => $"game.{field}"),
            Where = $"where category = {(int)ExternalGameCategoryEnum.ExternalgameSteam} & uid=\"{steamAppId}\""
        }, ExternalGameResult.Parser);
        if (externalGameResult.Externalgames.Count > 0)
        {
            return externalGameResult.Externalgames[0].Game;
        }

        return null;
    }

    public async Task<Game?> FindGameAsync(LibraryGame libraryGame)
    {
        if (libraryGame.LibraryService == LibraryService.Steam)
        {
            var steamAppId = libraryGame.ExternalId;
            var g = await GetSteamGame(steamAppId);
            if (g == null) return null;
            var gameType = g.Category switch
            {
                GameCategoryEnum.MainGame => GameType.Game,
                GameCategoryEnum.DlcAddon => GameType.Dlc,
                GameCategoryEnum.Mod => GameType.Mod,
                _ => GameType.Game
            };
            var details = new Game
            {
                IgdbId = g.Id,
                Name = g.Name,
                Description = g.Summary,
                CoverImage = new Image // TODO support multiple cover images
                {
                    IgdbImageId = g.Cover.ImageId,
                    Width = g.Cover.Width,
                    Height = g.Cover.Height,
                    ImageType = ImageType.Cover
                },
                Developers = (g.InvolvedCompanies ?? []).Where(c => c.Developer).Select(c => new Company
                {
                    IgdbId = c.Company.Id,
                    Name = c.Company.Name,
                    StartDate = c.Company.StartDate?.ToDateTimeOffsetOrNull(),
                    Logo = c.Company.Logo != null ? new Image
                        {
                            IgdbImageId = c.Company.Logo.ImageId,
                            Width = c.Company.Logo.Width,
                            Height = c.Company.Logo.Height,
                            ImageType = ImageType.Logo,
                        }
                        : null,
                }).ToList(),
                Publishers = (g.InvolvedCompanies?? []).Where(c => c.Publisher).Select(c => new Company
                {
                    IgdbId = c.Company.Id,
                    Name = c.Company.Name,
                    StartDate = c.Company.StartDate?.ToDateTimeOffsetOrNull(),
                    Logo = c.Company.Logo != null ? new Image
                        {
                            IgdbImageId = c.Company.Logo.ImageId,
                            Width = c.Company.Logo.Width,
                            Height = c.Company.Logo.Height,
                            ImageType = ImageType.Logo
                        }
                        : null,
                }).ToList(),
                Type = gameType,
                Releases = g.ReleaseDates.Select(d => new Release
                {
                    IgdbId = d.Id,
                    ReleaseDate = d.Date?.ToDateTimeOffsetOrNull(),
                    Platform = new Platform
                    {
                        IgdbId = d.Platform.Id,
                        Name = d.Platform.Name,
                        Logo = new Image
                        {
                            IgdbImageId = d.Platform.PlatformLogo.ImageId,
                            Width = d.Platform.PlatformLogo.Width,
                            Height = d.Platform.PlatformLogo.Height,
                            ImageType = ImageType.Logo
                        },
                    },
                    Region = d.Region switch
                    {
                       RegionRegionEnum.Asia => Region.Asia,
                       RegionRegionEnum.Australia => Region.Australia,
                       RegionRegionEnum.Brazil => Region.Brazil,
                       RegionRegionEnum.China => Region.China,
                       RegionRegionEnum.Europe => Region.Europe,
                       RegionRegionEnum.Japan => Region.Japan,
                       RegionRegionEnum.Korea => Region.Korea,
                       RegionRegionEnum.Worldwide => Region.Worldwide,
                       RegionRegionEnum.NorthAmerica => Region.NorthAmerica,
                       RegionRegionEnum.NewZealand => Region.NewZealand,
                       _ => Region.Unknown,
                    },
                }).ToList(),
                ReleaseDate = g.FirstReleaseDate.ToDateTimeOffsetOrNull()
            };
            if (g.Artworks.Count > 0)
            {
                details.BackgroundImage = new Image // TODO support multiple background images
                {
                    IgdbImageId = g.Artworks[0].ImageId,
                    Width = g.Artworks[0].Width,
                    Height = g.Artworks[0].Height,
                    ImageType = ImageType.Screenshot
                };
            }
            else if (g.Screenshots.Count > 0)
            {
                Console.WriteLine($"Using screenshot as background image for {details.Name}");
                details.BackgroundImage = new Image // TODO support multiple background images
                {
                    IgdbImageId = g.Screenshots[0].ImageId,
                    Width = g.Screenshots[0].Width,
                    Height = g.Screenshots[0].Height,
                    ImageType = ImageType.Screenshot
                };
            }
            return details;
        }

        throw new NotImplementedException("Cannot lookup game yet");
    }

    public Task<Stream> FetchImageAsync(Image image) => imageFetcher.FetchImageAsync(image);
}

public class IgdbError
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("status")]
    public int Status { get; set; }
    [JsonPropertyName("cause")]
    public string Cause { get; set; }
}
