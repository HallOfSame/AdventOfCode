using Helpers.Interfaces;
using Helpers.Structure;
using System.Globalization;

namespace PuzzleDays
{
    public class Day11 : StepExecutionPuzzle<Day11.ExecState>, IVisualizeText
    {
        public record ExecState(List<Stone> Stones, int BlinkCount, Dictionary<decimal, decimal> PartTwoStoneCounts);

        public override PuzzleInfo Info => new(2024, 11, "Plutonian Pebbles");

        public override bool ResetOnNewPart => true;

        protected override async Task<ExecState> LoadInitialState(string puzzleInput)
        {
            var split = puzzleInput.Trim().Split(" ");
            var stones = split.Select(x => new Stone { Number = decimal.Parse(x) }).ToList();
            return new ExecState(stones, 0, stones.GroupBy(x => x.Number).ToDictionary(x => x.Key, x => (decimal)x.Count()));
        }

        protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartOne()
        {
            var newStones = new List<Stone>();

            foreach(var stone in CurrentState.Stones)
            {
                newStones.AddRange(stone.RunRules());
            }

            CurrentState = new ExecState(newStones, CurrentState.BlinkCount + 1, CurrentState.PartTwoStoneCounts);

            return (CurrentState.BlinkCount == 25, newStones.Count.ToString());
        }

        protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartTwo()
        {
            var newStones = new Dictionary<decimal, decimal>();

            foreach(var (stoneNumber, multiplier) in CurrentState.PartTwoStoneCounts)
            {
                var numbersToAdd = GetStoneResult(stoneNumber);

                foreach(var number in numbersToAdd)
                {
                    if (newStones.TryGetValue(number, out var existingCount))
                    {
                        newStones[number] = existingCount + multiplier;
                    }
                    else
                    {
                        newStones[number] = multiplier;
                    }
                }
            }

            CurrentState = new ExecState(CurrentState.Stones, CurrentState.BlinkCount + 1, newStones);

            return (CurrentState.BlinkCount == 75, newStones.Values.Sum().ToString(CultureInfo.InvariantCulture));
        }

        public string GetText()
        {
            return string.Join(" ", CurrentState.Stones.Select(x => x.Number));
        }

        private decimal[] GetStoneResult(decimal stoneNumber)
        {
            if (stoneNumber == 0)
            {
                return
                [
                    1
                ];
            }

            var numberString = stoneNumber.ToString(CultureInfo.InvariantCulture);

            if (numberString.Length % 2 == 0)
            {
                var midpoint = numberString.Length / 2;

                return
                [
                    decimal.Parse(numberString[..midpoint]),
                    decimal.Parse(numberString[midpoint..])
                ];
            }

            return
            [
                stoneNumber * 2024m
            ];
        }

        public class Stone
        {
            public decimal Number {get;set; }

            public Stone[] RunRules()
            {
                if (Number == 0)
                {
                    return
                    [
                        new Stone { Number = 1m }
                    ];
                }

                var numberString = Number.ToString();

                if (numberString.Length % 2 == 0)
                {
                    var midpoint = numberString.Length / 2;

                    return
                    [
                        new Stone { Number = decimal.Parse(numberString[..midpoint])},
                        new Stone { Number = decimal.Parse(numberString[midpoint..])}
                    ];
                }

                return
                [
                    new Stone { Number = Number * 2024m}
                ];
            }
        }
    }
}
