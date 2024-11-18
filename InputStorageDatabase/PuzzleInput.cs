using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InputStorageDatabase;

public class PuzzleInput
{
    public int InputId { get; set; }
    public int Year { get; set; }
    public int Day { get; set; }
    public PuzzleInputType InputType { get; set; }
    public string Name { get; set; }
    public string Input { get; set; }
}

public enum PuzzleInputType
{
    Example = 1,
    Personal = 2
}

public class PuzzleInputTypeConfiguration : IEntityTypeConfiguration<PuzzleInput>
{
    public void Configure(EntityTypeBuilder<PuzzleInput> builder)
    {
        builder.ToTable("puzzle_input");

        builder.HasKey(x => x.InputId);
        builder.Property(x => x.InputId);
        builder.Property(x => x.Year);
        builder.Property(x => x.Day);
        builder.Property(x => x.Name);
        builder.Property(x => x.Input);
        builder.Property(x => x.InputType)
            .HasConversion<int>();

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}