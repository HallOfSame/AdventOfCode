using Helpers.Logging;
using Helpers.Structure;
using InputStorageDatabase;

namespace PuzzleDays;

public class Day03(ProgressLogger logger) : SingleExecutionPuzzle<Day03.State>
{
    public class State
    {
        public required List<string> BatteryBanks { get; set; } 
    }

    public override PuzzleInfo Info => new(2025, 3, "Lobby");

    protected override async Task<string> ExecutePuzzlePartOne()
    {
        var total = 0;

        foreach (var bank in InitialState.BatteryBanks)
        {
            var best = DfsFindBestOption(bank, 0, string.Empty);

            logger.LogProgress($"Best for {bank}: {best}");

            total += best;
        }

        return total.ToString();
    }

    private static int DfsFindBestOption(string currentBank, int currentIndex, string currentOption)
    {
        if (currentOption.Length == 2)
        {
            return int.Parse(currentOption);
        }

        if (currentIndex == currentBank.Length)
        {
            return 0;
        }

        var bestWithCurrentChar =
            DfsFindBestOption(currentBank, currentIndex + 1, $"{currentOption}{currentBank[currentIndex]}");

        var bestWithoutCurrentChar = DfsFindBestOption(currentBank, currentIndex + 1, currentOption);

        return Math.Max(bestWithCurrentChar, bestWithoutCurrentChar);
    }

    protected override async Task<string> ExecutePuzzlePartTwo()
    {
        var total = 0L;

        foreach (var bank in InitialState.BatteryBanks)
        {
            var best = FindBestPartTwo(bank);

            logger.LogProgress($"Best for {bank}: {best}");

            total += best;
        }

        return total.ToString();
    }

    private readonly List<char> digitsInOrder = ['9', '8', '7', '6', '5', '4', '3', '2', '1', '0'];

    private long FindBestPartTwo(string currentBank)
    {
        var digits = new List<int>(12);
        var upperBound = 0;

        // The highest number is always going to be the best leading digit we can make
        // And we can just work down constraining the string as we go
        for (var i = 12; i > 0; i--)
        {
            var requiredLengthRemaining = 12 - digits.Count;
            var stringToSearch = currentBank[upperBound..];
            // This is the latest digit we can pick that still lets us finish a bank
            var indexBound = stringToSearch.Length - requiredLengthRemaining;

            // Just brute forcing each digit in order
            foreach (var digitOption in digitsInOrder)
            {
                var indexOfOption = stringToSearch.IndexOf(digitOption);
                // If found and leaves enough to finish the rest, this is the best option to use here
                if (indexOfOption < 0 || indexOfOption > indexBound)
                {
                    continue;
                }

                // Add this digit
                digits.Add(int.Parse(digitOption.ToString()));
                // The upper bound is now the index after this
                // We + upperBound to keep it relative to the overall string, string indexOfOption is relative to the partial string
                upperBound = indexOfOption + 1 + upperBound;
                break;
            }
        }

        return long.Parse(string.Join(string.Empty, digits));
    }

    protected override async Task<State> LoadInputState(string puzzleInput, PuzzleInputType inputType)
    {
        var banks = puzzleInput.Trim()
            .Split("\r\n")
            .ToList();

        return new State
        {
            BatteryBanks = banks
        };
    }
}