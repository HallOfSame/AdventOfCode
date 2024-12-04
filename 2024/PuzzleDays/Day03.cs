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
                if (TryParseMulExpression(ref currentCharIndex, out var newExpression))
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
            throw new NotImplementedException();
        }

        private bool TryParseMulExpression(ref int currentIndex, out string mulExpression)
        {
            mulExpression = string.Empty;

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
