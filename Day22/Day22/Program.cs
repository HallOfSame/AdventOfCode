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

            // PT 2
            deckP1 = new Queue<int>();
            deckP2 = new Queue<int>();

            addingPlayerOne = true;

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

            RecursiveCombat(deckP1, deckP2, out var p1WinsGame);

            var winningDeckPt2 = p1WinsGame ? deckP1 : deckP2;

            winningScore = winningDeckPt2.Reverse()
                                         .Select((x,
                                                  idx) => x * (idx + 1))
                                         .Sum();

            Console.WriteLine($"Winning score PT2: {winningScore}");
        }

        private static HashSet<string> p1WinCodes = new HashSet<string>();

        private static HashSet<string> p2WinCodes = new HashSet<string>();

        private static void RecursiveCombat(Queue<int> deckP1,
                                            Queue<int> deckP2,
                                            out bool p1WinsGame)
        {
            var existingConfigurations = new HashSet<string>();

            var brokeEarly = false;

            p1WinsGame = false;

            do
            {
                // Brackets are important here
                // Otherwise we can't tell the difference between deck 1 2 3 and 12 3
                var currentConfig = string.Join("", deckP1.Select(x => $"[{x}]").Concat(deckP2.Select(x => x.ToString())));

                // Quick exit if we know the result from the decks
                if (p1WinCodes.Contains(currentConfig))
                {
                    p1WinsGame = true;
                    brokeEarly = true;
                    break;
                }

                if (p2WinCodes.Contains(currentConfig))
                {
                    p1WinsGame = false;
                    brokeEarly = true;
                    break;
                }

                if (existingConfigurations.Contains(currentConfig))
                {
                    break;
                }

                existingConfigurations.Add(currentConfig);

                var cardP1 = deckP1.Dequeue();
                var cardP2 = deckP2.Dequeue();

                bool p1WinsRound;

                if (deckP1.Count >= cardP1
                    && deckP2.Count >= cardP2)
                {
                    // Recurse to find winner
                    var subDeckP1 = new Queue<int>(deckP1.Take(cardP1));
                    var subDeckP2 = new Queue<int>(deckP2.Take(cardP2));

                    RecursiveCombat(subDeckP1, subDeckP2, out p1WinsRound);
                }
                else
                {
                    p1WinsRound = cardP1 > cardP2;
                }

                if (p1WinsRound)
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

            if (!brokeEarly)
            {
                if (deckP1.Any()
                    && deckP2.Any())
                {
                    // We broke early, P1 wins because of the exact config rule
                    p1WinsGame = true;
                }
                else
                {
                    p1WinsGame = deckP1.Any();
                }
            }

            // Save results we find
            if (p1WinsGame)
            {
                p1WinCodes.UnionWith(existingConfigurations);
            }
            else
            {
                p2WinCodes.UnionWith(existingConfigurations);
            }
        }
    }
}
