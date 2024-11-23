using System.ComponentModel.DataAnnotations.Schema;
using GameLibrary.Core;
using GameLibrary.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLibrary.SQLite;

public class LibraryContext : DbContext
{
    public DbSet<Platform> Platforms { get; set; } = null!;
    public DbSet<Image> Images { get; set; } = null!;
    public DbSet<ImageData> ImageData { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<Genre> Genres { get; set; } = null!;
    public DbSet<Theme> Themes { get; set; } = null!;
    public DbSet<Keyword> Keywords { get; set; } = null!;
    public DbSet<MultiplayerMode> MultiplayerModes { get; set; } = null!;
    public DbSet<LibraryGame> LibraryGames { get; set; } = null!;
    private string DbPath { get; } = Path.Combine(Configuration.DataFolderPath, "library.db");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // ensure data path exists
        Directory.CreateDirectory(Configuration.DataFolderPath);
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
        //optionsBuilder.LogTo(Console.WriteLine);
        //optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .HasMany(g => g.Developers).WithMany().UsingEntity(j =>
            {
                j.ToTable("GameDeveloper");
                j.IndexerProperty<Guid>("Id");
                j.HasKey("Id");
            });
        modelBuilder.Entity<Game>()
            .HasMany(g => g.Publishers).WithMany()
            .UsingEntity(j =>
            {
                j.ToTable("GamePublisher");
                j.IndexerProperty<Guid>("Id");
                j.HasKey("Id");
            });
        modelBuilder.Entity<Game>()
            .HasMany(g => g.Genres).WithMany().UsingEntity(j =>
            {
                j.ToTable("GameGenre");
                j.IndexerProperty<Guid>("Id");
                j.HasKey("Id");
            });
        modelBuilder.Entity<Game>()
            .HasMany(g => g.Keywords).WithMany().UsingEntity(j =>
            {
                j.ToTable("GameKeyword");
                j.IndexerProperty<Guid>("Id");
                j.HasKey("Id");
            });
        modelBuilder.Entity<Game>()
            .HasMany(g => g.MultiplayerModes).WithMany().UsingEntity(j =>
            {
                j.ToTable("GameMultiplayerMode");
                j.IndexerProperty<Guid>("Id");
                j.HasKey("Id");
            });
    }
}

public class ImageData
{
    public Guid Id { get; set; }
    public Guid ImageId { get; set; }
    public Image Image { get; set; } = null!;
    public byte[] Data { get; set; } = null!;
}
