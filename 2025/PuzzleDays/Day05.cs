using Helpers.Logging;
using Helpers.Structure;

namespace PuzzleDays;

public class Day05(ProgressLogger progress) : SingleExecutionPuzzle<Day05.State>
{
    public override PuzzleInfo Info => new(2025, 5, "Cafeteria");

    protected override async Task<string> ExecutePuzzlePartOne()
    {
        var result = 0;

        foreach (var available in InitialState.Available)
        {
            if (InitialState.FreshRanges.Any(x => available >= x.start && available <= x.end))
            {
                result++;
                progress.LogProgress($"Ingredient {available} is fresh");
            }
            else
            {
                progress.LogProgress($"Ingredient {available} is spoiled");
            }
        }

        return result.ToString();
    }

    protected override async Task<string> ExecutePuzzlePartTwo()
    {
        throw new NotImplementedException();
    }

    protected override async Task<State> LoadInputState(string puzzleInput, PuzzleInputType inputType)
    {
        var splits = puzzleInput.Split('\n');

        var readingRanges = true;
        var ranges = new List<(long, long)>();
        var available = new List<long>();

        foreach (var split in splits)
        {
            if (string.IsNullOrEmpty(split))
            {
                readingRanges = false;
                continue;
            }

            if (readingRanges)
            {
                var rangeSplit = split.Split('-');
                ranges.Add((long.Parse(rangeSplit[0]), long.Parse(rangeSplit[1])));
            }
            else
            {
                available.Add(long.Parse(split));
            }
        }

        return new State
        {
            FreshRanges = ranges,
            Available = available
        };
    }

    public class State
    {
        public required List<long> Available { get; set; }
        public required List<(long start, long end)> FreshRanges { get; set; }
    }
}