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
        throw new NotImplementedException();
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