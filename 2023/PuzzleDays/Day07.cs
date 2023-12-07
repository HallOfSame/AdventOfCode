using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.FileReaders;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day07 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            // Didn't want this in the input read because it's kind of solving the puzzle
            hands.ForEach(SetHandType);

            var orderedHands = hands.OrderByDescending(x => x, new HandComparer())
                .ToList();

            var scores = orderedHands.Select((x, idx) => x.Bid * (orderedHands.Count - idx));

            var result = scores.Sum()
                .ToString();

            return result;
        }

        private void SetHandType(Hand h)
        {
            var cardCounts = h.Cards.GroupBy(x => x)
                .ToArray();

            if (cardCounts.Length == 1)
            {
                h.HandType = HandType.FiveOfAKind;
                return;
            }

            if (cardCounts.Length == 2)
            {
                if (cardCounts.Any(x => x.Count() == 4))
                {
                    h.HandType = HandType.FourOfAKind;
                    return;
                }

                h.HandType = HandType.FullHouse;
                return;
            }

            if (cardCounts.Length == 3)
            {
                if (cardCounts.Any(x => x.Count() == 3))
                {
                    h.HandType = HandType.ThreeOfAKind;
                    return;
                }

                h.HandType = HandType.TwoPair;
                return;
            }

            if (cardCounts.Length == 4)
            {
                h.HandType = HandType.OnePair;
            }
            else
            {
                h.HandType = HandType.HighCard;
            }
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        public override async Task ReadInput()
        {
            var handLines = await new StringFileReader().ReadInputFromFile();

            hands = new List<Hand>(handLines.Count);

            foreach (var line in handLines)
            {
                var lineSplit = line.Split(' ');

                var bid = int.Parse(lineSplit[1]);

                var cards = lineSplit[0]
                    .Select(x => x switch
                    {
                        '2' => Card.Two,
                        '3' => Card.Three,
                        '4' => Card.Four,
                        '5' => Card.Five,
                        '6' => Card.Six,
                        '7' => Card.Seven,
                        '8' => Card.Eight,
                        '9' => Card.Nine,
                        'T' => Card.Ten,
                        'J' => Card.Jack,
                        'Q' => Card.Queen,
                        'K' => Card.King,
                        'A' => Card.Ace,
                        _ => throw new Exception($"Char {x} is not expected in a hand")
                    })
                    .ToArray();

                hands.Add(new Hand
                {
                    Cards = cards,
                    Bid = bid,
                });
            }
        }

        private List<Hand> hands;

        class Hand
        {
            public Card[] Cards { get; set; }

            public HandType HandType { get; set; }

            public int Bid { get; set; }
        }

        private const int CardsInAHand = 5;

        class HandComparer : IComparer<Hand>
        {
            public int Compare(Hand x, Hand y)
            {
                var handTypeComparison = x.HandType.CompareTo(y.HandType);

                if (handTypeComparison != 0)
                {
                    return handTypeComparison;
                }

                for (var i = 0; i < CardsInAHand; i++)
                {
                    if (x.Cards[i] != y.Cards[i])
                    {
                        return x.Cards[i] - y.Cards[i];
                    }
                }

                return 0;
            }
        }

        enum HandType
        {
            HighCard = 0,
            OnePair = 1,
            TwoPair = 2,
            ThreeOfAKind = 3,
            FullHouse = 4,
            FourOfAKind = 5,
            FiveOfAKind = 6
        }

        enum Card
        {
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8,
            Nine = 9,
            Ten = 10,
            Jack = 11,
            Queen = 12,
            King = 13,
            Ace = 14,
        }
    }
}
