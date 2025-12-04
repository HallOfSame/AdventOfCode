using Helpers.Structure;
using InputStorageDatabase;

namespace PuzzleDays;

public class Day02 : SingleExecutionPuzzle<Day02.State>
{
    public override PuzzleInfo Info => new(2025, 2, "Gift Shop");

    protected override async Task<string> ExecutePuzzlePartOne()
    {
        var result = 0L;

        foreach (var range in InitialState.Ranges)
        {
            var invalidIds = FindInvalidIdsInRange(range.start, range.end);

            invalidIds.ForEach(x => result += x);
        }

        return result.ToString();
    }

    private static List<long> FindInvalidIdsInRange(long start, long end)
    {
        var res = new List<long>();
        var current = start;

        while (current <= end)
        {
            var currString = current.ToString();

            if (currString.Length % 2 == 0)
            {
                if (currString[..(currString.Length / 2)] == currString[(currString.Length / 2)..])
                {
                    res.Add(current);
                }
            }

            current++;
        }

        return res;
    }

    protected override async Task<string> ExecutePuzzlePartTwo()
    {
        var result = 0L;

        foreach (var range in InitialState.Ranges)
        {
            var invalidIds = FindInvalidIdsInRangePart2(range.start, range.end);

            invalidIds.ForEach(x => result += x);
        }

        return result.ToString();
    }

    private static List<long> FindInvalidIdsInRangePart2(long start, long end)
    {
        var res = new List<long>();
        var current = start;

        while (current <= end)
        {
            var currString = current.ToString();
            if (IsInvalidIdPart2(currString))
            {
                res.Add(current);
            }

            current++;
        }

        return res;
    }

    private static bool IsInvalidIdPart2(string id)
    {
        var patternLengthOptions = Enumerable.Range(1, id.Length / 2);

        foreach (var length in patternLengthOptions)
        {
            if (CompletesPattern(id, id[..length], length, string.Empty))
            {
                return true;
            }
        }

        return false;
    }

    private static bool CompletesPattern(string id,
                                         string patternToCheck,
                                         int currentIndex,
                                         string currentPatternPart)
    {
        if (currentIndex == id.Length)
        {
            // Made it to the end, pattern is complete if we have no leftovers
            return currentPatternPart.Length == 0;
        }

        // Add the char at the current index
        var patternAtThisIndex = currentPatternPart + id[currentIndex];

        // If the length matches the pattern we check, see if it matches
        if (patternAtThisIndex.Length == patternToCheck.Length)
        {
            if (patternAtThisIndex != patternToCheck)
            {
                return false;
            }

            // Pattern has matched so far so see if it continues
            return CompletesPattern(id, patternToCheck, currentIndex + 1, "");
        }

        // Already wrong so we can exit quickly
        if (!patternToCheck.StartsWith(patternAtThisIndex))
        {
            return false;
        }

        // Keep going on the current pattern
        return CompletesPattern(id, patternToCheck, currentIndex + 1, patternAtThisIndex);
    }

    protected override Task<State> LoadInputState(string puzzleInput, PuzzleInputType inputType)
    {
        var rangeSplits = puzzleInput.Split(',');

        var ranges = rangeSplits.Select(x =>
            {
                var range = x.Split('-');
                var start = long.Parse(range[0]);
                var end = long.Parse(range[1]);

                return (start, end);
            })
            .ToList();

        return Task.FromResult(new State
        {
            Ranges = ranges
        });
    }

    public class State
    {
        public List<(long start, long end)> Ranges { get; set; } = [];
    }
}