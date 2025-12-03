namespace AoCRunner.Models;

public class PuzzleInfoUiModel
{
    public int GridColumn { get; init; }
    public int GridRow { get; init; }
    public required string PuzzleName { get; init; }
    public required int DayNumber { get; init; }
}