﻿using Helpers.Structure;
using InputStorageDatabase;

namespace PuzzleDays
{
    public class Day19 : SingleExecutionPuzzle<Day19.ExecState>
    {
        public record ExecState(List<string> Towels, List<string> Designs);

        public override PuzzleInfo Info => new(2024, 19, "Linen Layout");
        protected override async Task<ExecState> LoadInputState(string puzzleInput, PuzzleInputType inputType)
        {
            var lines = puzzleInput.Trim()
                .Split('\n');

            var towelPatterns = lines[0]
                .Split(", ")
                .ToList();

            var designs = lines.Skip(2)
                .ToList();

            return new ExecState(towelPatterns, designs);
        }

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var memo = new Dictionary<string, bool>();
            var designsWeCanMake = InitialState.Designs.Where(x => CanMakePattern(x, memo))
                .ToList();

            return designsWeCanMake.Count.ToString();
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            throw new NotImplementedException();
        }

        private bool CanMakePattern(string remainingDesign, Dictionary<string, bool> memo)
        {
            if (memo.TryGetValue(remainingDesign, out var existingResult))
            {
                return existingResult;
            }

            foreach (var towel in InitialState.Towels)
            {
                if (!remainingDesign.StartsWith(towel))
                {
                    continue;
                }

                if (towel.Length == remainingDesign.Length)
                {
                    memo[remainingDesign] = true;
                    return true;
                }

                var leftover = remainingDesign[towel.Length..];

                if (!CanMakePattern(leftover, memo))
                {
                    continue;
                }

                memo[remainingDesign] = true;
                return true;
            }

            memo[remainingDesign] = false;
            return false;
        }
    }
}
