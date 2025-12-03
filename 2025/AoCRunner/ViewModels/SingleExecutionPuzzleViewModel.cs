using System;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Helpers.Interfaces;
using InputStorageDatabase;
using System.Threading.Tasks;

namespace AoCRunner.ViewModels;

public partial class SingleExecutionPuzzleViewModel(ISingleExecutionPuzzle puzzle) : ViewModelBase
{
    public string Title => $"Day {puzzle.Info.Day} - {puzzle.Info.Name}";
    public string Input { get; set; } = string.Empty;

    [ObservableProperty]
    private string result = string.Empty;

    public async Task RunPartOne()
    {
        if (!await TryCatchInputLoad())
        {
            return;
        }

        var executionResult = await puzzle.ExecutePartOne();
        SetResultText(executionResult);
    }

    public async Task RunPartTwo()
    {
        if (!await TryCatchInputLoad())
        {
            return;
        }

        var executionResult = await puzzle.ExecutePartTwo();
        SetResultText(executionResult);
    }

    private void SetResultText(ExecutionResult executionResult)
    {
        var resultText = new StringBuilder();

        if (executionResult.Exception is not null)
        {
            resultText.Append($"Execution threw {executionResult.Exception}");
        }
        else
        {
            resultText.Append($"Calculated result {executionResult.Result}");
        }

        // TODO eventually show this on its own property
        resultText.Append($" in {executionResult.ElapsedTime}");

        Result = resultText.ToString();
    }

    private async Task<bool> TryCatchInputLoad()
    {
        try
        {
            await puzzle.LoadInput(Input, PuzzleInputType.Example);
        }
        catch (Exception ex)
        {
            Result = $"Failed to load input: {ex}";
            return false;
        }

        return true;
    }
}