using GameLibrary.Core.Models;

namespace GameLibrary.Core.Services;

public class LibrarySyncService(
    IDataSource dataSource,
    ILibraryGameRepository libraryGameRepository,
    IMetadataRepository metadataRepository,
    IImageRepository imageRepository,
    ICompanyRepository companyRepository)
{
    public LibrarySyncTask CreateSyncTask(IGameLibrary library)
    {
        return new LibrarySyncTask
        {
            Library = library,
            DataSource = dataSource,
            LibraryGameRepository = libraryGameRepository,
            MetadataRepository = metadataRepository,
            ImageRepository = imageRepository,
            CompanyRepository = companyRepository,
        };
    }
}

public class LibrarySyncTask : ObservableTask<LibrarySyncStatus>
{
    protected override LibrarySyncStatus Status { get; } = new();
    public required IDataSource DataSource { get; init; }
    public required IGameLibrary Library { get; init; }
    public required ILibraryGameRepository LibraryGameRepository { get; init; }
    public required IMetadataRepository MetadataRepository { get; init; }
    public required IImageRepository ImageRepository { get; init; }
    public required ICompanyRepository CompanyRepository { get; init; }

    public override async Task Run()
    {
        Status.GameCount = await Library.GetGameCountAsync();
        NotifyStatus();
        foreach (var game in await Library.GetGamesAsync())
        {
            await SyncLibraryGameAsync(game);
            Status.ImportedGameCount++;
            NotifyStatus();
        }
        NotifyCompleted();
    }

    private async Task<LibraryGame?> SyncLibraryGameAsync(LibraryGame libraryGame)
    {
        var existing = await LibraryGameRepository.FindExistingAsync(libraryGame);
        if (existing != null) return existing;
        var game = await DataSource.FindGameAsync(libraryGame);
        if (game != null) game = await SyncGameAsync(game);
        if (game != null) libraryGame.Release = ChooseRelease(libraryGame, game);
        if (game == null) return null;
        await LibraryGameRepository.UpdateAsync(libraryGame);
        return libraryGame;
    }

    private static Release ChooseRelease(LibraryGame libraryGame, Game game)
    {
        foreach (var release in game.Releases)
        {
            if (release.Platform.Name == libraryGame.Release?.Platform.Name)
            {
                return release;
            }
        }

        return game.Releases[0];
    }

    private async Task<Game> SyncGameAsync(Game game)
    {
        var existing = await MetadataRepository.FindExistingGameAsync(game);
        if (existing != null) return existing;
        if (game.BackgroundImage != null) game.BackgroundImage = await SyncImageAsync(game.BackgroundImage);
        if (game.CoverImage != null) game.CoverImage = await SyncImageAsync(game.CoverImage);
        for (var i = 0; i < game.Developers.Count; i++) game.Developers[i] = await SyncCompanyAsync(game.Developers[i]);
        for (var i = 0; i < game.Publishers.Count; i++) game.Publishers[i] = await SyncCompanyAsync(game.Publishers[i]);
        for (var i = 0; i < game.Genres.Count; i++) game.Genres[i] = await SyncGenreAsync(game.Genres[i]);
        for (var i = 0; i < game.Keywords.Count; i++) game.Keywords[i] = await SyncKeywordAsync(game.Keywords[i]);
        for (var i = 0; i < game.MultiplayerModes.Count; i++) game.MultiplayerModes[i] = await SyncMultiplayerModeAsync(game.MultiplayerModes[i]);
        for (var i = 0; i < game.Themes.Count; i++) game.Themes[i] = await SyncThemeAsync(game.Themes[i]);
        foreach (var release in game.Releases) release.Platform = await SyncPlatformAsync(release.Platform);
        DeduplicateCompanies(game);
        DeduplicatePlatforms(game);
        await MetadataRepository.UpdateGameAsync(game);
        return game;
    }

    private static void DeduplicatePlatforms(Game game)
    {
        var platforms = new Dictionary<Guid, Platform>();
        foreach (var platform in game.Releases.Select(r => r.Platform)) platforms.TryAdd(platform.Id, platform);
        foreach (var release in game.Releases) release.Platform = platforms[release.Platform.Id];
    }

    private static void DeduplicateCompanies(Game game)
    {
        var companies = new Dictionary<Guid, Company>();
        foreach (var company in game.Developers) companies.TryAdd(company.Id, company);
        foreach (var company in game.Publishers) companies.TryAdd(company.Id, company);
        for (var i = 0; i < game.Developers.Count; i++) game.Developers[i] = companies[game.Developers[i].Id];
        for (var i = 0; i < game.Publishers.Count; i++) game.Publishers[i] = companies[game.Publishers[i].Id];
    }

    private async Task<Platform> SyncPlatformAsync(Platform platform)
    {
        var existing = await MetadataRepository.FindExistingPlatformAsync(platform);
        if (existing != null) return existing;
        await MetadataRepository.UpdatePlatformAsync(platform);
        return platform;
    }

    private async Task<Theme> SyncThemeAsync(Theme theme)
    {
        var existing = await MetadataRepository.FindExistingThemeAsync(theme);
        if (existing != null) return existing;
        await MetadataRepository.UpdateThemeAsync(theme);
        return theme;
    }

    private async Task<MultiplayerMode> SyncMultiplayerModeAsync(MultiplayerMode multiplayerMode)
    {
        var existing = await MetadataRepository.FindExistingMultiplayerModeAsync(multiplayerMode);
        if (existing != null) return existing;
        await MetadataRepository.UpdateMultiplayerModeAsync(multiplayerMode);
        return multiplayerMode;
    }

    private async Task<Keyword> SyncKeywordAsync(Keyword keyword)
    {
        var existing = await MetadataRepository.FindExistingKeywordAsync(keyword);
        if (existing != null) return existing;
        await MetadataRepository.UpdateKeywordAsync(keyword);
        return keyword;
    }

    private async Task<Genre> SyncGenreAsync(Genre genre)
    {
        var existing = await MetadataRepository.FindExistingGenreAsync(genre);
        if (existing != null) return existing;
        await MetadataRepository.UpdateGenreAsync(genre);
        return genre;
    }

    private async Task<Company> SyncCompanyAsync(Company company)
    {
        var existing = await CompanyRepository.FindExistingAsync(company);
        if (existing != null) return existing;
        if (company.Logo != null) company.Logo = await SyncImageAsync(company.Logo);
        await CompanyRepository.Add(company);
        return company;
    }

    private async Task<Image> SyncImageAsync(Image image)
    {
        var existing = await ImageRepository.FindExistingAsync(image);
        if (existing != null) return existing;
        return await ImageRepository.UploadAsync(image, await DataSource.FetchImageAsync(image));
    }
}

public class LibrarySyncStatus
{
    public int? GameCount { get; set; }
    public int ImportedGameCount { get; set; }
}
