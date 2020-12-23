using System;
using System.Collections.Generic;
using System.Linq;

namespace Day23
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = "";

            var cupList = new List<Cup>();

            foreach (var cupName in input.ToCharArray())
            {
                var cupId = int.Parse(cupName.ToString());

                cupList.Add(new Cup(cupId));
            }

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

            var cups = cupList.ToDictionary(x => x.Label,
                                            x => x);

            var currentCup = cupList.First();

            var numberOfTurns = 100;

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

                // Get destination
                var destinationCup = cupList.Where(x => x.Label < currentCup.Label && x.Next != null && x.Prev != null && x.Next.Label != lastCup.Label)
                                            .OrderByDescending(x => x.Label)
                                            .FirstOrDefault();

                if (destinationCup == null)
                {
                    // Wrap around if not one lower
                    destinationCup = cupList.Where(x => x.Label > currentCup.Label && x.Next != null && x.Prev != null && x.Next.Label != lastCup.Label)
                                            .OrderByDescending(x => x.Label)
                                            .First();
                }

                // Place the 3 cups after the destination
                var destinationNext = destinationCup.Next;

                destinationCup.Next = firstCup;
                firstCup.Prev = destinationCup;

                destinationNext.Prev = lastCup;
                lastCup.Next = destinationNext;

                // Select next current cup
                currentCup = currentCup.Next;
            }

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
    }

    public class Cup
    {
        public Cup(int label)
        {
            Label = label;
        }

        public int Label { get; }

        public Cup Next { get; set; }

        public Cup Prev { get; set; }
    }
}
