using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day04 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            return scratchcards.Select(GetScratchcardScore)
                .Sum()
                .ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        public override async Task ReadInput()
        {
            scratchcards = await new ScratchcardReader().ReadInputFromFile();
        }

        private int GetScratchcardScore(Scratchcard scratchcard)
        {
            var matchingNumbers = scratchcard.YourNumbers.Count(num => scratchcard.WinningNumbers.Contains(num));

            return (int)Math.Pow(2, matchingNumbers - 1);
        }

        private List<Scratchcard> scratchcards;

        class Scratchcard
        {
            public HashSet<int> WinningNumbers { get; set; }

            public HashSet<int> YourNumbers { get; set; }
        }

        class ScratchcardReader : FileReader<Scratchcard>
        {
            protected override Scratchcard ProcessLineOfFile(string line)
            {
                var numberSide = line.Split(": ")[1];

                var numberSplit = numberSide.Split(" | ");

                var winningNumbers = numberSplit[0]
                    .Split(' ')
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(int.Parse)
                    .ToHashSet();

                var yourNumbers = numberSplit[1]
                    .Split(' ')
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(int.Parse)
                    .ToHashSet();

                return new Scratchcard
                {
                    WinningNumbers = winningNumbers,
                    YourNumbers = yourNumbers,
                };
            }
        }
    }
}
