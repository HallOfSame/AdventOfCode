using CommunityToolkit.Mvvm.ComponentModel;
using Helpers.Interfaces;
using InputStorageDatabase;
using System.Threading.Tasks;

namespace AoCRunner.ViewModels;

public partial class SingleExecutionPuzzleViewModel(ISingleExecutionPuzzle puzzle) : ViewModelBase
{
    public string Input { get; set; } = string.Empty;

    [ObservableProperty]
    private string _result = string.Empty;

    public async Task RunPartOne()
    {
        await puzzle.LoadInput(Input, PuzzleInputType.Example);
        var result = await puzzle.ExecutePartOne();

        Result = $"Got result {result.Result} in {result.ElapsedTime}";
    }

    public async Task RunPartTwo()
    {
        await puzzle.LoadInput(Input, PuzzleInputType.Example);
        var result = await puzzle.ExecutePartTwo();

        Result = $"Got result {result.Result} in {result.ElapsedTime}";
    }
}