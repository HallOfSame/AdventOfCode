using System.Globalization;
using Helpers.Structure;

namespace PuzzleDays;

public class Day06 : SingleExecutionPuzzle<Day06.State>
{
    public enum Operator
    {
        Add,
        Multiply
    }

    public override PuzzleInfo Info => new(2025, 6, "Trash Compactor");

    protected override async Task<string> ExecutePuzzlePartOne()
    {
        checked
        {
            var total = 0m;

            foreach (var problem in InitialState.Problems)
            {
                if (problem.Operator == Operator.Add)
                {
                    total += problem.Operands.Sum();
                }
                else
                {
                    total += problem.Operands.Aggregate(1m, (curr, next) => curr * next);
                }
            }

            return total.ToString(CultureInfo.InvariantCulture);
        }
    }

    protected override async Task<string> ExecutePuzzlePartTwo()
    {
        throw new NotImplementedException();
    }

    protected override async Task<State> LoadInputState(string puzzleInput, PuzzleInputType inputType)
    {
        var lines = puzzleInput.Split('\n');

        var firstLineSplit = lines[0]
            .Split(default(char[]), StringSplitOptions.RemoveEmptyEntries);

        var problems = new Problem[firstLineSplit.Length];
        var problemCount = problems.Length;

        for (var i = 0; i < problemCount; i++)
            problems[i] = new Problem
            {
                Operands = [int.Parse(firstLineSplit[i])]
            };

        foreach (var line in lines.Skip(1)
                     .Take(lines.Length - 2))
        {
            var lineSplit = line.Split(default(char[]), StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < problemCount; i++)
                problems[i]
                    .Operands.Add(int.Parse(lineSplit[i]));
        }

        var lastLineSplit = lines[^1]
            .Split(default(char[]), StringSplitOptions.RemoveEmptyEntries);

        for (var i = 0; i < problemCount; i++)
            problems[i].Operator = lastLineSplit[i] switch
            {
                "+" => Operator.Add,
                "*" => Operator.Multiply,
                _ => throw new InvalidOperationException($"Invalid operator {lastLineSplit[i]}")
            };

        return new State
        {
            Problems = problems.ToList()
        };
    }

    public class State
    {
        public required List<Problem> Problems { get; set; }
    }

    public class Problem
    {
        public required List<int> Operands { get; set; }
        public Operator Operator { get; set; }
    }
}