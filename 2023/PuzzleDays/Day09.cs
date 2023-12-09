using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.FileReaders;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day09 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var result = histories.Select(GetNextValue)
                .Sum();

            return result.ToString();
        }

        private int GetNextValue(int[] currentList)
        {
            if (currentList.All(x => x == 0))
            {
                return 0;
            }

            var diffs = new int[currentList.Length - 1];

            for (var i = 0; i < currentList.Length - 1; i++)
            {
                var secondValue = currentList[i + 1];
                var firstValue = currentList[i];

                diffs[i] = secondValue - firstValue;
            }

            var nextValueDiff = GetNextValue(diffs);

            return currentList[^1] + nextValueDiff;
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var result = histories.Select(GetPrevValue)
                .Sum();

            return result.ToString();
        }

        private int GetPrevValue(int[] currentList)
        {
            if (currentList.All(x => x == 0))
            {
                return 0;
            }

            var diffs = new int[currentList.Length - 1];

            for (var i = 0; i < currentList.Length - 1; i++)
            {
                var secondValue = currentList[i + 1];
                var firstValue = currentList[i];

                diffs[i] = secondValue - firstValue;
            }

            var prevValueDiff = GetPrevValue(diffs);

            return currentList[0] - prevValueDiff;
        }

        public override async Task ReadInput()
        {
            var strings = await new StringFileReader().ReadInputFromFile();

            histories = strings.Select(x => x.Split()
                    .Select(int.Parse)
                    .ToArray())
                .ToList();
        }

        private List<int[]> histories;
    }
}
