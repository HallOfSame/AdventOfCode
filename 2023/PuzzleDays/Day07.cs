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
            hands.ForEach(h => h.HandType = GetHandType(h.Cards));

            var orderedHands = hands.OrderByDescending(x => x, new HandComparer(false))
                .ToList();

            var scores = orderedHands.Select((x, idx) => x.Bid * (orderedHands.Count - idx));

            var result = scores.Sum()
                .ToString();

            return result;
        }

        private HandType GetHandType(Card[] cards)
        {
            var cardCounts = cards.GroupBy(x => x)
                .ToArray();

            if (cardCounts.Length == 1)
            {
                return HandType.FiveOfAKind;
            }

            if (cardCounts.Length == 2)
            {
                if (cardCounts.Any(x => x.Count() == 4))
                {
                    return HandType.FourOfAKind;
                }

                return HandType.FullHouse;
            }

            if (cardCounts.Length == 3)
            {
                if (cardCounts.Any(x => x.Count() == 3))
                {
                    return HandType.ThreeOfAKind;
                }

                return HandType.TwoPair;
            }

            if (cardCounts.Length == 4)
            {
                return HandType.OnePair;
            }
            
            return HandType.HighCard;
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            hands.ForEach(h => h.HandType = GetHandTypeForPart2(h));

            var orderedHands = hands.OrderByDescending(x => x, new HandComparer(true))
                .ToList();

            var scores = orderedHands.Select((x, idx) => x.Bid * (orderedHands.Count - idx));

            var sum = scores.Sum();

            var result = sum
                .ToString();

            return result;
        }

        private HandType GetHandTypeForPart2(Hand h)
        {
            if (h.Cards.All(x => x != Card.Jack) || h.HandType == HandType.FiveOfAKind)
            {
                // No jokers, no change
                // Or 5 of a kind and we can't get better
                return h.HandType;
            }

            // Try each hand with jokers as the other cards in that hand
            // There isn't really a scenario where having a joker be two different cards is better than having them all be the same card
            // There's also no scenario where you'd want them to be different than the cards in your hand currently
            var uniqueCards = h.Cards.Distinct();

            var updatedType = h.HandType;

            foreach (var uniqueCard in uniqueCards)
            {
                var cloneHand = h.Cards.Select(x => x == Card.Jack ? uniqueCard : x)
                    .ToArray();

                // Some hacky casting to use Math.Max
                updatedType = (HandType)Math.Max((int)updatedType, (int)GetHandType(cloneHand));
            }

            return updatedType;
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
            private readonly bool isPartTwo;

            public HandComparer(bool isPartTwo)
            {
                this.isPartTwo = isPartTwo;
            }

            public int Compare(Hand x, Hand y)
            {
                var handTypeComparison = x.HandType.CompareTo(y.HandType);

                if (handTypeComparison != 0)
                {
                    return handTypeComparison;
                }

                for (var i = 0; i < CardsInAHand; i++)
                {
                    var xCard = x.Cards[i];
                    var yCard = y.Cards[i];

                    if (isPartTwo)
                    {
                        xCard = xCard == Card.Jack ? Card.Joker : xCard;
                        yCard = yCard == Card.Jack ? Card.Joker : yCard;
                    }

                    if (xCard != yCard)
                    {
                        return xCard - yCard;
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
            Joker = 1,
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
