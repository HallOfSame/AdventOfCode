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
        var combinedRanges = CondenseRanges(InitialState.FreshRanges);

        // End - start is 1 short for each range to add how many ranges we have
        var total = combinedRanges.Select(x => x.end - x.start)
            .Sum() + combinedRanges.Count;

        return total.ToString();
    }

    private static List<(long start, long end)> CondenseRanges(
        IEnumerable<(long start, long end)> ranges)
    {
        // Order ranges by start
        var sorted = ranges
            .OrderBy(r => r.start)
            .ToList();

        var result = new List<(long start, long end)>
        {
            sorted[0]
        };

        // For the rest
        foreach (var current in sorted.Skip(1))
        {
            // Get the current end of our results
            // Since they're in order this is the one we could possibly extend
            var last = result[^1];

            // Overlap or adjacent
            if (current.start <= last.end)
            {
                // Overwrite (can't assign because tuples are value objects)
                result[^1] = (last.start, Math.Max(last.end, current.end));
            }
            // New section in the overall ranges
            else
            {
                result.Add(current);
            }
        }

        return result;
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