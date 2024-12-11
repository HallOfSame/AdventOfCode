using Helpers.Interfaces;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day11 : StepExecutionPuzzle<Day11.ExecState>, IVisualizeText
    {
        public record ExecState(List<Stone> Stones, int BlinkCount);

        public override PuzzleInfo Info => new(2024, 11, "Plutonian Pebbles");

        public override bool ResetOnNewPart => false;

        protected override async Task<ExecState> LoadInitialState(string puzzleInput)
        {
            var split = puzzleInput.Trim().Split(" ");
            var stones = split.Select(x => new Stone { Number = decimal.Parse(x) }).ToList();
            return new ExecState(stones, 0);
        }

        protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartOne()
        {
            var newStones = new List<Stone>();

            foreach(var stone in CurrentState.Stones)
            {
                newStones.AddRange(stone.RunRules());
            }

            CurrentState = new ExecState(newStones, CurrentState.BlinkCount + 1);

            return (CurrentState.BlinkCount == 25, newStones.Count.ToString());
        }

        protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartTwo()
        {
            throw new NotImplementedException();
        }

        public string GetText()
        {
            return string.Join(" ", CurrentState.Stones.Select(x => x.Number));
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
