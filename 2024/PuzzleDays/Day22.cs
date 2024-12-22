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
                result += CalculateSecretNumberAfterRounds(startingNumber, 2000).Last();
            }

            return result.ToString();
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            var allMonkeyLookups = new List<Dictionary<(int, int, int, int), int>>();

            foreach (var startingNumber in InitialState.SecretNumbers)
            {
                // Get the list of secret numbers
                var secretNumbersForThisMonkey = CalculateSecretNumberAfterRounds(startingNumber, 2000);
                // Convert them to prices
                var pricesForMonkey = secretNumbersForThisMonkey.Select(x => (int)(x % 10))
                    .ToList();
                // And then price changes
                var priceChanges = pricesForMonkey.Skip(1)
                    .Select((curr, idx) => curr - pricesForMonkey[idx])
                    .ToList();

                // Map out the price the first team each sequence is found
                var sequencePrices = new Dictionary<(int, int, int, int), int>();

                for (var i = 3; i < priceChanges.Count; i++)
                {
                    var sequence = (priceChanges[i - 3], priceChanges[i - 2], priceChanges[i - 1], priceChanges[i]);
                    // prices list is 1 longer than change list
                    var priceAtThisTime = pricesForMonkey[i + 1];

                    // Only the first time counts
                    sequencePrices.TryAdd(sequence, priceAtThisTime);
                }

                allMonkeyLookups.Add(sequencePrices);
            }

            var bestTotal = 0;
            var bestSequence = (0, 0, 0, 0);
            // Just check every sequence that exists and find the best one
            var allSequencesToCheck = allMonkeyLookups.SelectMany(x => x.Keys)
                .ToHashSet();

            foreach (var sequenceToTest in allSequencesToCheck)
            {
                var totalForThisSequence = allMonkeyLookups.Sum(monkey => monkey.GetValueOrDefault(sequenceToTest, 0));

                if (totalForThisSequence <= bestTotal)
                {
                    continue;
                }

                bestTotal = totalForThisSequence;
                bestSequence = sequenceToTest;
            }

            return bestTotal.ToString();
        }
        
        private static List<long> CalculateSecretNumberAfterRounds(long secretNumber, int numberOfRounds)
        {
            var secretNumbers = new List<long>
            {
                secretNumber
            };

            for (var round = 1; round <= numberOfRounds; round++)
            {
                secretNumber = CalculateSecretNumber(secretNumber);
                secretNumbers.Add(secretNumber);
            }

            return secretNumbers;
        }

        private static long CalculateSecretNumber(long secretNumber)
        {
            var firstMix = secretNumber * 64L;
            secretNumber = MixAndPrune(secretNumber, firstMix);
            var secondMix = secretNumber / 32L;
            secretNumber = MixAndPrune(secretNumber, secondMix);
            var thirdMix = secretNumber * 2048L;
            secretNumber = MixAndPrune(secretNumber, thirdMix);
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
