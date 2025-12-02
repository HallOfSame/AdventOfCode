using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using InputStorageDatabase;
using PuzzleDays;

namespace AoCRunner.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Input { get; set; } = string.Empty;

    [ObservableProperty]
    private string _result = string.Empty;

    public Day01 Puzzle = new();

    public async Task RunPartOne()
    {
        await Puzzle.LoadInput(Input, PuzzleInputType.Example);
        var result = await Puzzle.ExecutePartOne();
            
        Result = $"Got result {result.Result} in {result.ElapsedTime}";
    }

    public async Task RunPartTwo()
    {
        await Puzzle.LoadInput(Input, PuzzleInputType.Example);
        var result = await Puzzle.ExecutePartTwo();

        Result = $"Got result {result.Result} in {result.ElapsedTime}";
    }
}
