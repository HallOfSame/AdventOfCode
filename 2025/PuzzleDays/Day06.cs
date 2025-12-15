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
        checked
        {
            var total = 0m;

            foreach (var problem in InitialState.Problems)
            {
                if (problem.Operator == Operator.Add)
                {
                    total += problem.Part2Operands.Sum();
                }
                else
                {
                    total += problem.Part2Operands.Aggregate(1m, (curr, next) => curr * next);
                }
            }

            return total.ToString(CultureInfo.InvariantCulture);
        }
    }

    protected override async Task<State> LoadInputState(string puzzleInput, PuzzleInputType inputType)
    {
        var lines = puzzleInput.Split('\n');

        var firstLineSplit = lines[0]
            .Split(default(char[]), StringSplitOptions.RemoveEmptyEntries);

        var problems = new Problem[firstLineSplit.Length];
        var problemCount = problems.Length;

        for (var i = 0; i < problemCount; i++)
        {
            problems[i] = new Problem
            {
                Operands = [int.Parse(firstLineSplit[i])]
            };
        }

        foreach (var line in lines.Skip(1)
                     .Take(lines.Length - 2))
        {
            var lineSplit = line.Split(default(char[]), StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < problemCount; i++)
            {
                problems[i]
                    .Operands.Add(int.Parse(lineSplit[i]));
            }
        }

        var lastLineSplit = lines[^1]
            .Split(default(char[]), StringSplitOptions.RemoveEmptyEntries);

        for (var i = 0; i < problemCount; i++)
        {
            problems[i].Operator = lastLineSplit[i] switch
            {
                "+" => Operator.Add,
                "*" => Operator.Multiply,
                _ => throw new InvalidOperationException($"Invalid operator {lastLineSplit[i]}")
            };
        }

        // Track the offset when reading from a given line
        var currentOffset = 0;

        // For each problem
        foreach (var problem in problems)
        {
            // The length of the longest operand (width) gives us the count of part 2 operands
            var longestOperandLength = problem.Operands.Max()
                .ToString()
                .Length;

            // Start with empty strings for each
            var v2Operands = Enumerable.Range(0, problem.Operands.Count)
                .Select(_ => string.Empty)
                .ToList();

            // For every part 2 operand
            for (var operandIndex = 0; operandIndex < longestOperandLength; operandIndex++)
            {
                // Grab the next character (blank or digit) for this operand
                // The count of operands from part 1 (# of lines tall) tells us how many digits we could possibly have
                for (var digit = 0; digit < problem.Operands.Count; digit++)
                {
                    // The digit tells us which line to look at
                    // The index tells us how far over to look (when adding the offset for the start of the current problem)
                    v2Operands[operandIndex] += lines[digit][currentOffset + operandIndex];
                }
            }

            // Push the base offset over to the next operand
            currentOffset += longestOperandLength + 1;

            // Then just parse, ignoring empty lines (code assumes we have the same number of operands, but we can have less)
            problem.Part2Operands = v2Operands.Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(int.Parse)
                .ToList();
        }

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

        public List<int> Part2Operands { get; set; }
        public Operator Operator { get; set; }
    }
}