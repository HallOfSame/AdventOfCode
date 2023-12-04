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
            return scratchcards.Select(GetScratchcardPointScore)
                .Sum()
                .ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var copies = new Dictionary<int, int>();

            foreach (var card in scratchcards)
            {
                CalculateCopies(card, copies);
            }

            return (scratchcards.Count + copies.Values.Sum()).ToString();
        }

        public override async Task ReadInput()
        {
            scratchcards = await new ScratchcardReader().ReadInputFromFile();
        }

        private void CalculateCopies(Scratchcard currentCard, Dictionary<int, int> copies)
        {
            var matchingNumbers = currentCard.YourNumbers.Count(num => currentCard.WinningNumbers.Contains(num));

            copies.TryGetValue(currentCard.CardNumber, out var instancesOfCurrentCard);

            // Include the 1 original card
            instancesOfCurrentCard += 1;

            for (var i = 1; i <= matchingNumbers; i++)
            {
                var copyIndex = currentCard.CardNumber + i;

                if (!copies.TryGetValue(copyIndex, out var currentCopies))
                {
                    copies[copyIndex] = instancesOfCurrentCard;
                }
                else
                {
                    copies[copyIndex] = currentCopies + instancesOfCurrentCard;
                }
            }
        }

        private int GetScratchcardPointScore(Scratchcard scratchcard)
        {
            var matchingNumbers = scratchcard.YourNumbers.Count(num => scratchcard.WinningNumbers.Contains(num));

            return (int)Math.Pow(2, matchingNumbers - 1);
        }

        private List<Scratchcard> scratchcards;

        class Scratchcard
        {
            public int CardNumber { get; set; }

            public HashSet<int> WinningNumbers { get; set; }

            public HashSet<int> YourNumbers { get; set; }
        }

        class ScratchcardReader : FileReader<Scratchcard>
        {
            protected override Scratchcard ProcessLineOfFile(string line)
            {
                var lineSplit = line.Split(": ");

                var cardNumber = int.Parse(lineSplit[0]
                    .Replace("Card ", string.Empty));

                var numberSide = lineSplit[1];

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
                    CardNumber = cardNumber,
                    WinningNumbers = winningNumbers,
                    YourNumbers = yourNumbers,
                };
            }
        }
    }
}
