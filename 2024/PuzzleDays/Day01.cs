using Helpers.Structure;
using InputStorageDatabase;

namespace PuzzleDays;

public class Day01 : SingleExecutionPuzzle<Day01.ExecState>
{
    public override PuzzleInfo Info => new(2024, 1, "Historian Hysteria");

    protected override async Task<ExecState> LoadInputState(string puzzleInput, PuzzleInputType inputType)
    {
        var left = new List<int>();
        var right = new List<int>();

        foreach (var line in puzzleInput.Split("\n"))
        {
            var splitLine = line.Split(default(char[]), StringSplitOptions.RemoveEmptyEntries);

            left.Add(int.Parse(splitLine[0]));
            right.Add(int.Parse(splitLine[1]));
        }

        return new ExecState(left, right);
    }

    protected override async Task<string> ExecutePuzzlePartOne()
    {
        var sortedLeft = InitialState.Left.OrderBy(x => x)
            .ToList();
        var sortedRight = InitialState.Right.OrderBy(x => x)
            .ToList();

        var result = sortedLeft.Select((leftVal, idx) =>
            {
                var rightVal = sortedRight[idx];
                return Math.Abs(leftVal - rightVal);
            })
            .Sum();

        return result.ToString();
    }

    protected override async Task<string> ExecutePuzzlePartTwo()
    {
        var rightCount = InitialState.Right.GroupBy(x => x)
            .ToDictionary(x => x.Key, x => x.Count());

        var result = InitialState.Left.Select(leftVal => leftVal * rightCount.GetValueOrDefault(leftVal, 0))
            .Sum();

        return result.ToString();
    }

    public record ExecState(List<int> Left, List<int> Right);
}