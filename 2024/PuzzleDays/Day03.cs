using System.Diagnostics;
using System.Text.RegularExpressions;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day03 : SingleExecutionPuzzle<Day03.ExecState>
    {
        public record ExecState(string MemoryValue);

        public override PuzzleInfo Info => new(2024, 03, "Mull It Over");
        protected override async Task<Day03.ExecState> LoadInputState(string puzzleInput)
        {
            return new ExecState(puzzleInput);
        }

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var validMulInstructions = new List<string>();
            var currentCharIndex = 0;

            do
            {
                if (TryParseMulExpression(ref currentCharIndex, out var newExpression, out _))
                {
                    validMulInstructions.Add(newExpression);
                }
            } while (currentCharIndex < InitialState.MemoryValue.Length);

            return validMulInstructions.Select(CalculateMulExpression)
                .Sum()
                .ToString();
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            var validMulInstructions = new List<(string expr, int startIdx)>();
            var currentCharIndex = 0;

            // Get all the expressions
            do
            {
                if (TryParseMulExpression(ref currentCharIndex, out var newExpression, out var startIndex))
                {
                    validMulInstructions.Add((newExpression, startIndex));
                }
            } while (currentCharIndex < InitialState.MemoryValue.Length);

            var doIndexes = new List<int>
            {
                // Start with 0 because they are enabled until we reach a don't
                0
            };
            var dontIndexes = new List<int>();

            // Find the index of any do() and don't() command
            for (currentCharIndex = 0; currentCharIndex < InitialState.MemoryValue.Length; currentCharIndex++)
            {
                if (currentCharIndex + 4 >= InitialState.MemoryValue.Length)
                {
                    break;
                }

                if (InitialState.MemoryValue.Substring(currentCharIndex, 4) == "do()")
                {
                    doIndexes.Add(currentCharIndex);
                    continue;
                }

                if (currentCharIndex + 7 >= InitialState.MemoryValue.Length)
                {
                    continue;
                }

                if (InitialState.MemoryValue.Substring(currentCharIndex, 7) == "don't()")
                {
                    dontIndexes.Add(currentCharIndex);
                }
            }

            // Then use that to build a list of valid ranges
            var validRanges = new List<(int start, int end)>();

            foreach (var doIdx in doIndexes)
            {
                var followingDont = dontIndexes.FirstOrDefault(x => x > doIdx);

                if (followingDont == 0)
                {
                    followingDont = int.MaxValue;
                }

                validRanges.Add((doIdx, followingDont));
            }

            // Filter down the valid instructions to the enabled ones
            var enabledMulInstructions =
                validMulInstructions.Where(x => validRanges.Any(range => range.start <= x.startIdx &&
                                                                         range.end >= x.startIdx));

            return enabledMulInstructions.Select(x => CalculateMulExpression(x.expr))
                .Sum()
                .ToString();
        }

        private bool TryParseMulExpression(ref int currentIndex, out string mulExpression, out int startIndex)
        {
            mulExpression = string.Empty;
            startIndex = currentIndex;

            if (currentIndex + 4 >= InitialState.MemoryValue.Length || InitialState.MemoryValue.Substring(currentIndex, 4) != "mul(")
            {
                currentIndex += 1;
                return false;
            }

            currentIndex += 4;

            var firstDigit = string.Empty;

            do
            {
                var currentChar = InitialState.MemoryValue[currentIndex];

                if (char.IsDigit(currentChar))
                {
                    firstDigit += InitialState.MemoryValue[currentIndex];
                }
                else
                {
                    if (currentChar != ',')
                    {
                        return false;
                    }

                    break;
                }

                currentIndex++;

                if (firstDigit.Length == 3)
                {
                    break;
                }
            } while (true);

            if (firstDigit.Length == 3 && InitialState.MemoryValue[currentIndex] != ',')
            {
                return false;
            }

            currentIndex++;

            var secondDigit = string.Empty;

            do
            {
                var currentChar = InitialState.MemoryValue[currentIndex];

                if (char.IsDigit(currentChar))
                {
                    secondDigit += InitialState.MemoryValue[currentIndex];
                }
                else
                {
                    if (currentChar != ')')
                    {
                        return false;
                    }

                    break;
                }

                currentIndex++;

                if (secondDigit.Length == 3)
                {
                    break;
                }
            } while (true);

            if (secondDigit.Length == 3 && InitialState.MemoryValue[currentIndex] != ')')
            {
                return false;
            }

            mulExpression = $"mul({firstDigit},{secondDigit})";
            currentIndex++;
            return true;
        }

        private Regex MulRegex = new(@"mul\((\d+),(\d+)\)");

        private int CalculateMulExpression(string expression)
        {
            var match = MulRegex.Match(expression);

            var valOne = int.Parse(match.Groups[1].Value);
            var valTwo = int.Parse(match.Groups[2].Value);

            return valOne * valTwo;
        }
    }
}
