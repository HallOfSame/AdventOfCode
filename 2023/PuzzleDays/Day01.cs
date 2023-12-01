using Helpers.FileReaders;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day01 : ProblemBase
    {
        private List<string> documentLines;

        protected override Task<string> SolvePartOneInternal()
        {
            var sum = 0;

            foreach (var line in documentLines)
            {
                var firstDigit = '0';
                var secondDigit = '0';

                for (var i = 0; i < line.Length; i++)
                {
                    if (char.IsDigit(line[i]))
                    {
                        firstDigit = line[i];
                        break;
                    }
                }

                for (var i = line.Length - 1; i >= 0; i--)
                {
                    if (char.IsDigit(line[i]))
                    {
                        secondDigit = line[i];
                        break;
                    }
                }

                var thisLineValue = int.Parse($"{firstDigit}{secondDigit}");

                sum += thisLineValue;
            }

            return Task.FromResult(sum.ToString());
        }

        protected override Task<string> SolvePartTwoInternal()
        {
            var sum = 0;

            // I don't love how long this is, but it do go pretty quick. Only 7ms for part 2
            char? GetDigitStartingAtIndex(string line, int startIndex, int lineLength)
            {
                if (char.IsDigit(line[startIndex]))
                {
                    return line[startIndex];
                }

                var remainingChar = lineLength - startIndex;

                if (remainingChar <= 2)
                {
                    return null;
                }

                if (line[startIndex] == 'o' && line[startIndex + 1] == 'n' && line[startIndex + 2] == 'e')
                {
                    return '1';
                }
                if (line[startIndex] == 't' && line[startIndex + 1] == 'w' && line[startIndex + 2] == 'o')
                {
                    return '2';
                }
                if (line[startIndex] == 's' && line[startIndex + 1] == 'i' && line[startIndex + 2] == 'x')
                {
                    return '6';
                }

                if (remainingChar <= 3)
                {
                    return null;
                }

                if (line[startIndex] == 'f' && line[startIndex + 1] == 'o' && line[startIndex + 2] == 'u' && line[startIndex + 3] == 'r')
                {
                    return '4';
                }
                if (line[startIndex] == 'f' && line[startIndex + 1] == 'i' && line[startIndex + 2] == 'v' && line[startIndex + 3] == 'e')
                {
                    return '5';
                }
                if (line[startIndex] == 'n' && line[startIndex + 1] == 'i' && line[startIndex + 2] == 'n' && line[startIndex + 3] == 'e')
                {
                    return '9';
                }

                if (remainingChar <= 4)
                {
                    return null;
                }

                if (line[startIndex] == 't' && line[startIndex + 1] == 'h' && line[startIndex + 2] == 'r' && line[startIndex + 3] == 'e' && line[startIndex + 4] == 'e')
                {
                    return '3';
                }
                if (line[startIndex] == 'e' && line[startIndex + 1] == 'i' && line[startIndex + 2] == 'g' && line[startIndex + 3] == 'h' && line[startIndex + 4] == 't')
                {
                    return '8';
                }
                if (line[startIndex] == 's' && line[startIndex + 1] == 'e' && line[startIndex + 2] == 'v' && line[startIndex + 3] == 'e' && line[startIndex + 4] == 'n')
                {
                    return '7';
                }

                return null;
            }

            foreach (var line in documentLines)
            {
                var firstDigit = ' ';
                var mostRecentDigit = '0';

                var lineLength = line.Length;

                for (var i = 0; i < lineLength; i++)
                {
                    var digitAtI = GetDigitStartingAtIndex(line, i, lineLength);

                    if (digitAtI is null)
                    {
                        continue;
                    }

                    if (firstDigit == ' ')
                    {
                        firstDigit = digitAtI.Value;
                    }

                    mostRecentDigit = digitAtI.Value;
                }

                var thisLineValue = int.Parse($"{firstDigit}{mostRecentDigit}");

                sum += thisLineValue;
            }

            return Task.FromResult(sum.ToString());
        }

        public override async Task ReadInput()
        {
            documentLines = await new StringFileReader().ReadInputFromFile();
        }
    }
}