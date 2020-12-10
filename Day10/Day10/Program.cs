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

            var forcedConnections = input.Where((x,
                                                 idx) =>
                                                {
                                                    if (idx - 1 < 0)
                                                    {
                                                        // First item in the list only cares about if we have to have it to connect to the second
                                                        return input[idx + 1] - x == 3;
                                                    }
                                                    else if (idx + 1 >= input.Count)
                                                    {
                                                        // Final one always needs to connect
                                                        return true;
                                                    }

                                                    // If the next item is 3 away from the second we always have to hook up the first adapter
                                                    var requiredToConnectToNext = input[idx + 1] - x == 3;

                                                    // Similarly if the prev item is 3 away we must always connect our current adapter
                                                    var requiredToConnectToPrev = input[idx - 1] - x == -3;

                                                    return requiredToConnectToNext || requiredToConnectToPrev;
                                                })
                                         .ToList();

            var optionalAdapters = input.Where(x => !forcedConnections.Contains(x))
                                        .ToList();

            // Math time
            // We want to figure out from our list of optional adapters "how many ways could we have just N of them plugged in" out of X optional adapters
            // Thanks to S/O this is a binomial coefficient
            // Formula: C = (X!) / (N! * (X-N)!)
            var combinations = 0;

            decimal GetFactorial(int num)
            {
                if (num == 0)
                {
                    return 1;
                }

                return Enumerable.Range(1,
                                        num)
                                 .Aggregate(1m,
                                            (fac,
                                             val) => fac * val);
            }

            var xFactorial = GetFactorial(optionalAdapters.Count);

            for (var i = 0; i <= optionalAdapters.Count; i++)
            {
                var nFactorial = GetFactorial(i);

                var xMinusNFactorial = GetFactorial(optionalAdapters.Count - i);

                var combinationsAtI = (xFactorial) / (nFactorial * xMinusNFactorial);

                Console.WriteLine($"Combinations at {i}: {combinationsAtI}");

                combinations += (int)combinationsAtI;
            }
            
            Console.WriteLine($"Combinations of hookups for our device {combinations}.");
        }
    }
}
