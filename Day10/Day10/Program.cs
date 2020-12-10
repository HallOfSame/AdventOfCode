using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Day10
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var input = await new InputReader().ReadInputFromFile();


            // PT 1
            input.Sort();

            var currentJolt = 0;
            var diff1Count = 0;
            var diff3Count = 0;

            foreach (var jolt in input)
            {
                var joltDifference = jolt - currentJolt;

                var valid = joltDifference <= 3;

                if (!valid)
                {
                    throw new InvalidOperationException($"Can't use all because a difference of {joltDifference} was found.");
                }

                currentJolt = jolt;

                if (joltDifference == 1)
                {
                    diff1Count++;
                }
                else if (joltDifference == 3)
                {
                    diff3Count++;
                }
            }

            // Last adapter to input always adds an extra diff of 3
            diff3Count++;

            Console.WriteLine($"Diff 1 {diff1Count} with Diff 3 {diff3Count}. Puzzle result {diff1Count * diff3Count}.");

            // PT 2

            var deviceJolt = input.Last() + 3;

            // We know there's a way to hook them all together in a line from PT 1
            // Can we find a minimal hook up then get the total based on the number of optional ones?

            currentJolt = 0;

            var minimalJolts = new List<int>();

            for (var i = 0; i < input.Count; i++)
            {
                var possibleHookups = input.Skip(i)
                                           .Take(3)
                                           .ToList();

                var largestValidHook = possibleHookups.Last(x => x - currentJolt <= 3);

                i += possibleHookups.IndexOf(largestValidHook);

                currentJolt = largestValidHook;

                minimalJolts.Add(largestValidHook);
            }

            var optionalAdapters = input.Where(x => !minimalJolts.Contains(x))
                                        .ToList();

            // Math time
            // We want to figure out from our list of optional adapters "how many ways could we have just N of them plugged in" out of X optional adapters
            // Thanks to S/O this is a binomial coefficient
            // Formula: C = (X!) / (N! * (X-N)!)
            var combinations = 0;

            int GetFactorial(int num)
            {
                if (num == 0)
                {
                    return 1;
                }

                return Enumerable.Range(1,
                                        num)
                                 .Aggregate(1,
                                            (fac,
                                             val) => fac * val);
            }

            var xFactorial = GetFactorial(optionalAdapters.Count);

            for (var i = 0; i < optionalAdapters.Count; i++)
            {
                var nFactorial = GetFactorial(i);

                var xMinusNFactorial = GetFactorial(optionalAdapters.Count - i);

                var combinationsAtI = (xFactorial) / (nFactorial * xMinusNFactorial);

                combinations += combinationsAtI;
            }

            // Add one for the minimal combo we already calculated
            combinations++;

            Console.WriteLine($"Combinations of hookups for our device {combinations}.");
        }
    }
}
