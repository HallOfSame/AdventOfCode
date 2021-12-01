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

            var deviceJolt = input.Max() + 3;

            input.Reverse();

            var joltPaths = new Dictionary<int, long>
                            {
                                {
                                    deviceJolt, 1
                                }
                            };

            foreach (var jolt in input)
            {
                joltPaths.TryGetValue(jolt + 1,
                                      out var pathsToOneJolt);

                joltPaths.TryGetValue(jolt + 2,
                                      out var pathsToTwoJolts);

                joltPaths.TryGetValue(jolt + 3,
                                      out var pathsToThreeJolts);

                joltPaths[jolt] = pathsToOneJolt + pathsToTwoJolts + pathsToThreeJolts;
            }

            var validStarts = input.Where(x => x <= 3)
                                   .ToList();

            var combinations = validStarts.Select(x => joltPaths[x])
                                          .Sum();
            
            Console.WriteLine($"Combinations of hookups for our device {combinations}.");
        }
    }
}
