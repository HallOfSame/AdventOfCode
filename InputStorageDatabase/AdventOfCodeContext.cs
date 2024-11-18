using Microsoft.EntityFrameworkCore;

namespace InputStorageDatabase;

public class AdventOfCodeContext : DbContext
{
    public DbSet<PuzzleInput> PuzzleInputs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbFile = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                               "adventofcode.db");

        optionsBuilder.UseSqlite($"Data Source={dbFile}");

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PuzzleInputTypeConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}