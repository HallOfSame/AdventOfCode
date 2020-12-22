using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day22
{
    class Program
    {
        public static void Main(string[] args)
        {
            var fileLines = File.ReadAllLines("PuzzleInput.txt");

            var deckP1 = new Queue<int>();
            var deckP2 = new Queue<int>();

            var addingPlayerOne = true;

            foreach(var line in fileLines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    addingPlayerOne = false;
                }

                if (!int.TryParse(line, out var cardValue))
                {
                    continue;
                }

                if (addingPlayerOne)
                {
                    deckP1.Enqueue(cardValue);
                }
                else
                {
                    deckP2.Enqueue(cardValue);
                }
            }

            
            do
            {
                var cardP1 = deckP1.Dequeue();
                var cardP2 = deckP2.Dequeue();

                if (cardP1 > cardP2)
                {
                    deckP1.Enqueue(cardP1);
                    deckP1.Enqueue(cardP2);
                }
                else
                {
                    deckP2.Enqueue(cardP2);
                    deckP2.Enqueue(cardP1);
                }
            }
            while (deckP1.Any()
                   && deckP2.Any());

            var winningDeck = deckP1.Any() ? deckP1 : deckP2;

            var winningScore = winningDeck.Reverse()
                                          .Select((x,
                                                   idx) => x * (idx + 1))
                                          .Sum();

            Console.WriteLine($"Winning score: {winningScore}");
        }
    }
}
