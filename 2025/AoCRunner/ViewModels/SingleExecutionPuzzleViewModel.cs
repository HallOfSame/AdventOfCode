using System;
using System.Collections.ObjectModel;
using System.Text;
using Helpers.Interfaces;
using InputStorageDatabase;
using System.Threading.Tasks;
using Helpers.Logging;
using AoCRunner.Views;

namespace AoCRunner.ViewModels;

public sealed class SingleExecutionPuzzleViewModel : ViewModelBase, IDisposable
{
    private readonly ISingleExecutionPuzzle puzzle;
    private readonly ProgressLogger logger;

    public SingleExecutionPuzzleViewModel(ISingleExecutionPuzzle puzzle, ProgressLogger logger)
    {
        this.puzzle = puzzle;
        this.logger = logger;

        logger.OnProgress += OnProgress;
    }

    public string Title => $"Day {puzzle.Info.Day} - {puzzle.Info.Name}";
    public string Input { get; set; } = string.Empty;

    public ObservableCollection<ProgressText> Progress { get; } = [];

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

    public void Visualize()
    {
        if (puzzle is not IVisualize2d visualizablePuzzle)
        {
            return;
        }

        var visualize = new VisualizeWindow
        {
            DataContext = new VisualizeViewModel(visualizablePuzzle.GetCoordinates())
        };

        visualize.Show();
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

        resultText.Append($" in {executionResult.ElapsedTime}");

        AddProgressText(resultText.ToString());
    }

    private async Task<bool> TryCatchInputLoad()
    {
        ClearProgress();

        try
        {
            AddProgressText("Loading puzzle input...");
            await puzzle.LoadInput(Input, PuzzleInputType.Example);
            AddProgressText("Input loaded...");
        }
        catch (Exception ex)
        {
            AddProgressText($"Failed to load input: {ex}", MessageType.Error);
            return false;
        }

        return true;
    }

    private void AddProgressText(string text, MessageType type = MessageType.Normal)
    {
        Progress.Add(new ProgressText
        {
            Text = text,
            Type = type
        });
    }

    private void ClearProgress()
    {
        Progress.Clear();
    }

    private void OnProgress(object? sender, (string message, MessageType type) info)
    {
        Progress.Add(new ProgressText
        {
            Text = info.message,
            Type = info.type
        });
    }

    public void Dispose()
    {
        logger.OnProgress -= OnProgress;
    }
}

public class ProgressText
{
    public required string Text { get; init; }
    public required MessageType Type { get; init; }
}