using Helpers.Structure;
using InputStorageDatabase;

namespace PuzzleDays
{
    public class Day22 : SingleExecutionPuzzle<Day22.ExecState>
    {
        public record ExecState(List<int> SecretNumbers);

        public override PuzzleInfo Info => new(2024, 22, "Monkey Market");
        protected override async Task<ExecState> LoadInputState(string puzzleInput, PuzzleInputType inputType)
        {
            var numbers = puzzleInput.Trim()
                .Split('\n')
                .Select(int.Parse)
                .ToList();

            return new ExecState(numbers);
        }

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var result = 0L;

            foreach (var startingNumber in InitialState.SecretNumbers)
            {
                result += CalculateSecretNumber(startingNumber, 2000);
            }

            return result.ToString();
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            throw new NotImplementedException();
        }

        private static long CalculateSecretNumber(long secretNumber, int numberOfRounds)
        {
            for (var round = 1; round <= numberOfRounds; round++)
            {
                var firstMix = secretNumber * 64L;
                secretNumber = MixAndPrune(secretNumber, firstMix);
                var secondMix = secretNumber / 32L;
                secretNumber = MixAndPrune(secretNumber, secondMix);
                var thirdMix = secretNumber * 2048L;
                secretNumber = MixAndPrune(secretNumber, thirdMix);
            }

            return secretNumber;
        }

        private static long MixAndPrune(long secretNumber, long mixValue)
        {
            var mixed = secretNumber ^ mixValue;
            var pruned = mixed % 16777216;
            return pruned;
        }
    }
}
