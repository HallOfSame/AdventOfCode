using System;
using System.Collections.Generic;
using System.Linq;

namespace Day23
{
    internal class Program
    {
        #region Class Methods

        public static void Main(string[] args)
        {
            PartOne();
            PartTwo();
        }

        public static void PartOne()
        {
            var cupList = GetCupListFromInput();

            LinkCups(cupList);

            var cups = cupList.ToDictionary(x => x.Label,
                                            x => x);

            var currentCup = cupList.First();

            var numberOfTurns = 100;

            RunCupSim(numberOfTurns,
                      currentCup,
                      cups,
                      false);

            var cupOne = cups[1];

            var moveResult = string.Empty;

            var next = cupOne.Next;

            while (next.Label != 1)
            {
                moveResult += next.Label.ToString();
                next = next.Next;
            }

            // PT 1
            Console.WriteLine($"Result of {numberOfTurns} turns: {moveResult}.");
        }

        public static void PartTwo()
        {
            var cupList = GetCupListFromInput();

            var maxCup = cupList.Max(x => x.Label);

            foreach (var number in Enumerable.Range(maxCup + 1,
                                                    1_000_000 - maxCup))
            {
                cupList.Add(new Cup(number));
            }

            LinkCups(cupList);

            var cups = cupList.ToDictionary(x => x.Label,
                                            x => x);

            var currentCup = cupList.First();

            var numberOfTurns = 10_000_000;

            RunCupSim(numberOfTurns,
                      currentCup,
                      cups,
                      true);

            var cupOne = cups[1];

            var firstTarget = cupOne.Next;
            var secondTarget = firstTarget.Next;

            // PT 2
            Console.WriteLine($"Result of {numberOfTurns} turns: {(long)firstTarget.Label * (long)secondTarget.Label}.");
        }

        private static List<Cup> GetCupListFromInput()
        {
            var input = "467528193";

            var cupList = new List<Cup>();

            foreach (var cupName in input.ToCharArray())
            {
                var cupId = int.Parse(cupName.ToString());

                cupList.Add(new Cup(cupId));
            }

            return cupList;
        }

        private static void LinkCups(List<Cup> cupList)
        {
            for (var i = 0; i < cupList.Count; i++)
            {
                var cupAtIndex = cupList[i];

                if (i == 0)
                {
                    cupAtIndex.Prev = cupList.Last();
                }
                else
                {
                    cupAtIndex.Prev = cupList[i - 1];
                }

                if (i == cupList.Count - 1)
                {
                    cupAtIndex.Next = cupList.First();
                }
                else
                {
                    cupAtIndex.Next = cupList[i + 1];
                }
            }
        }

        private static void RunCupSim(int numberOfTurns,
                                      Cup currentCup,
                                      Dictionary<int, Cup> cups,
                                      bool partTwo)
        {
            var currentTurn = numberOfTurns;

            while (currentTurn > 0)
            {
                currentTurn--;

                // Get first and last cups we'll move
                // 3 cups to right of current
                var firstCup = currentCup.Next;
                var lastCup = currentCup.Next.Next.Next;

                // Remove from the circle
                currentCup.Next = lastCup.Next;
                firstCup.Prev = null;
                lastCup.Next = null;

                var destinationLabel = currentCup.Label - 1;

                var labelsToMove = new[]
                                   {
                                       firstCup.Label,
                                       firstCup.Next.Label,
                                       lastCup.Label
                                   };

                while (true)
                {
                    if (destinationLabel < 1)
                    {
                        destinationLabel = partTwo
                                               ? 1_000_000
                                               : cups.Keys.Max();
                    }

                    if (labelsToMove.Contains(destinationLabel))
                    {
                        destinationLabel--;
                    }
                    else
                    {
                        break;
                    }
                }

                // Get destination
                var destinationCup = cups[destinationLabel];

                // Place the 3 cups after the destination
                var destinationNext = destinationCup.Next;

                destinationCup.Next = firstCup;
                firstCup.Prev = destinationCup;

                destinationNext.Prev = lastCup;
                lastCup.Next = destinationNext;

                // Select next current cup
                currentCup = currentCup.Next;
            }
        }

        #endregion
    }

    public class Cup
    {
        #region Constructors

        public Cup(int label)
        {
            Label = label;
        }

        #endregion

        #region Instance Properties

        public int Label { get; }

        public Cup Next { get; set; }

        public Cup Prev { get; set; }

        #endregion
    }
}