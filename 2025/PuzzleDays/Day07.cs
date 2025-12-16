using Helpers.Drawing;
using Helpers.FileReaders;
using Helpers.Interfaces;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays;

public class Day07 : StepExecutionPuzzle<Day07.State>, IVisualize2d
{
    public override PuzzleInfo Info => new(2025, 7, "Laboratories");
    public override bool ResetOnNewPart => false;

    public DrawableCoordinate[] GetCoordinates()
    {
        var drawables = CurrentState.Map.ToDrawableCoordinates();

        foreach (var beam in CurrentState.BeamLocations)
        {
            drawables.First(x => x.X == beam.X && x.Y == beam.Y)
                .Text = "|";
        }

        return drawables;
    }

    protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartOne()
    {
        if (CurrentState.BeamLocations.Count == 0)
        {
            return (true, CurrentState.NumberOfSplits.ToString());
        }

        var nextStepLocations = new HashSet<Coordinate>();

        foreach (var beam in CurrentState.BeamLocations)
        {
            var nextStep = beam.GetDirection(Direction.South);

            if (!CurrentState.Map.TryGetValue(nextStep, out var atNextStep))
            {
                // Next step down is not on the map
                continue;
            }

            if (atNextStep == '.')
            {
                // Simple case
                nextStepLocations.Add(nextStep);
            }
            else if (atNextStep == '^')
            {
                // Split
                var left = nextStep.GetDirection(Direction.East);
                var right = nextStep.GetDirection(Direction.West);

                if (CurrentState.Map.ContainsKey(left))
                {
                    nextStepLocations.Add(left);
                }

                if (CurrentState.Map.ContainsKey(right))
                {
                    nextStepLocations.Add(right);
                }

                CurrentState.NumberOfSplits++;
            }
        }

        CurrentState.BeamLocations = nextStepLocations;

        return (false, CurrentState.NumberOfSplits.ToString());
    }

    protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartTwo()
    {
        throw new NotImplementedException();
    }

    protected override async Task<State> LoadInitialState(string puzzleInput)
    {
        var map = (await new GridFileReader().ReadFromString(puzzleInput))
            .ToDictionary(x => x.Coordinate, x => x.Value);

        var startLocation = map.Single(x => x.Value == 'S').Key;

        return new State
        {
            Map = map,
            BeamLocations = [startLocation]
        };
    }

    public class State
    {
        public required Dictionary<Coordinate, char> Map { get; set; }
        public required HashSet<Coordinate> BeamLocations { get; set; }
        public int NumberOfSplits { get; set; }
    }
}