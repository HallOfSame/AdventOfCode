using Helpers.Drawing;
using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Interfaces;
using Helpers.Maps;
using Helpers.Structure;
using InputStorageDatabase;

namespace PuzzleDays;

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
        throw new NotImplementedException();
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