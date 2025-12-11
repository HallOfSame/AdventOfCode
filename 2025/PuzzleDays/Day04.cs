using Helpers.Drawing;
using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Interfaces;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays;

// TODO make a step execution from this
public class Day04 : SingleExecutionPuzzle<Day04.State>, IVisualize2d
{
    public override PuzzleInfo Info => new(2025, 4, "Printing Department");

    public DrawableCoordinate[] GetCoordinates()
    {
        return
        [
            .. InitialState.Grid.Select(x => new DrawableCoordinate
            {
                Color = "White",
                Text = x.Value.ToString(),
                X = x.Key.X,
                Y = x.Key.Y
            })
        ];
    }

    protected override async Task<string> ExecutePuzzlePartOne()
    {
        return InitialState.Grid
            .Count(x => x.Value == '@' && x.Key.GetNeighbors(true).Count(n => InitialState.Grid.TryGetValue(n, out var nChar) && nChar == '@') < 4).ToString();
    }

    protected override async Task<string> ExecutePuzzlePartTwo()
    {
        var removedCount = 0;

        while(true)
        {
            var toRemove = InitialState.Grid
            .Where(x => x.Value == '@' && x.Key.GetNeighbors(true).Count(n => InitialState.Grid.TryGetValue(n, out var nChar) && nChar == '@') < 4).ToList();

            if (toRemove.Count == 0)
            {
                break;
            }

            removedCount += toRemove.Count;

            toRemove.ForEach(x => InitialState.Grid[x.Key] = '.');
        }

        return removedCount.ToString();
    }

    protected override async Task<State> LoadInputState(string puzzleInput, PuzzleInputType inputType)
    {
        var gridData = await new GridFileReader().ReadFromString(puzzleInput);

        return new State
        {
            Grid = gridData.ToDictionary(x => x.Coordinate, x => x.Value)
        };
    }

    public class State
    {
        public required Dictionary<Coordinate, char> Grid { get; set; }
    }
}