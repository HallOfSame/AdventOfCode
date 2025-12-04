using System;
using System.Collections.Generic;
using System.Text;
using Helpers.Structure;
using InputStorageDatabase;

namespace PuzzleDays
{
    public class Day02 : SingleExecutionPuzzle<Day02.State>
    {
        public override PuzzleInfo Info => new(2025, 2, "Gift Shop");
        
        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var result = 0L;

            foreach (var range in InitialState.Ranges)
            {
                var invalidIds = FindInvalidIdsInRange(range.start, range.end);

                invalidIds.ForEach(x => result += x);
            }

            return result.ToString();
        }

        private static List<long> FindInvalidIdsInRange(long start, long end)
        {
            var res = new List<long>();
            var current = start;

            while (current <= end)
            {
                var currString = current.ToString();

                if (currString.Length % 2 == 0)
                {
                    if (currString[..(currString.Length / 2)] == currString[(currString.Length / 2)..])
                    {
                        res.Add(current);
                    }
                }

                current++;
            }

            return res;
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            throw new NotImplementedException();
        }

        protected override Task<State> LoadInputState(string puzzleInput, PuzzleInputType inputType)
        {
            var rangeSplits = puzzleInput.Split(',');

            var ranges = rangeSplits.Select(x =>
                {
                    var range = x.Split('-');
                    var start = long.Parse(range[0]);
                    var end = long.Parse(range[1]);

                    return (start, end);
                })
                .ToList();

            return Task.FromResult(new State
            {
                Ranges = ranges
            });
        }

        public class State
        {
            public List<(long start, long end)> Ranges { get; set; } = [];
        }
    }
}
