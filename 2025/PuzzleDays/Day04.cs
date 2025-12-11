using Helpers.Drawing;
using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Interfaces;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays;

// TODO make a step execution from this
public class Day04 : StepExecutionPuzzle<Day04.State>, IVisualize2d
{
    public override PuzzleInfo Info => new(2025, 4, "Printing Department");

    public DrawableCoordinate[] GetCoordinates()
    {
        return
        [
            .. CurrentState.Grid.Select(x => new DrawableCoordinate
            {
                Color = "White",
                Text = CurrentState.RemovedThisRound.Contains(x.Key) ? "x" : x.Value.ToString(),
                X = x.Key.X,
                Y = x.Key.Y
            })
        ];
    }
    
    public override bool ResetOnNewPart => false;

    protected override async Task<State> LoadInitialState(string puzzleInput)
    {
        var gridData = await new GridFileReader().ReadFromString(puzzleInput);

        return new State
        {
            Grid = gridData.ToDictionary(x => x.Coordinate, x => x.Value)
        };
    }

    protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartOne()
    {
        var count = InitialState.Grid
            .Count(x => x.Value == '@' && x.Key.GetNeighbors(true).Count(n => InitialState.Grid.TryGetValue(n, out var nChar) && nChar == '@') < 4).ToString();

        return (true, count);
    }

    protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartTwo()
    {
        var toRemove = CurrentState.Grid
            .Where(x => x.Value == '@' && x.Key.GetNeighbors(true)
                       .Count(n => CurrentState.Grid.TryGetValue(n, out var nChar) && nChar == '@') < 4)
            .ToList();

        CurrentState.RemovedThisRound = toRemove.Select(x => x.Key)
            .ToHashSet();

        toRemove.ForEach(x => CurrentState.Grid[x.Key] = '.');

        CurrentState.RemovedCount += toRemove.Count;

        return (toRemove.Count == 0, CurrentState.RemovedCount.ToString());
    }

    public class State
    {
        public required Dictionary<Coordinate, char> Grid { get; set; }
        public int RemovedCount { get; set; }
        public HashSet<Coordinate> RemovedThisRound { get; set; } = [];
    }
}