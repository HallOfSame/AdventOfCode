using System;
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
        }
    }
}
