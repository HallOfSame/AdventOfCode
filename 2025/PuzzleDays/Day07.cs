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
        // This half won't actually be a step-by-step process
        return (true, GetNumberOfTimelines(CurrentState.Map.Single(x => x.Value == 'S')
                                               .Key,
                                           [])
            .ToString());
    }

    private long GetNumberOfTimelines(Coordinate currentParticlePosition, Dictionary<Coordinate, long> memo)
    {
        if (memo.TryGetValue(currentParticlePosition, out var knownResult))
        {
            // Didn't test if this was needed, but it can't hurt
            return knownResult;
        }

        if (!CurrentState.Map.ContainsKey(currentParticlePosition))
        {
            // Edge case, we split and were immediately off the map
            memo[currentParticlePosition] = 1;
            return 1;
        }

        var originalCallPosition = currentParticlePosition;

        do
        {
            var nextPosition = currentParticlePosition.GetDirection(Direction.South);

            if (!CurrentState.Map.TryGetValue(nextPosition, out var atNext))
            {
                // Base case, we got to the end of a run
                memo[originalCallPosition] = 1;
                return 1;
            }

            if (atNext == '.')
            {
                // Nothing interesting to do at this point
                currentParticlePosition = nextPosition;
                continue;
            }

            if (atNext == '^')
            {
                break;
            }
        } while (true);

        // If we got here, we hit the opportunity to split
        var splitLeft = currentParticlePosition.GetDirection(Direction.East);
        var leftSplitTimelines = GetNumberOfTimelines(splitLeft, memo);
        var splitRight = currentParticlePosition.GetDirection(Direction.West);
        var rightSplitTimelines = GetNumberOfTimelines(splitRight, memo);
        memo[originalCallPosition] = leftSplitTimelines + rightSplitTimelines;

        return leftSplitTimelines + rightSplitTimelines;
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

        /// <summary>
        /// Used by part 1.
        /// </summary>
        public int NumberOfSplits { get; set; }
    }
}