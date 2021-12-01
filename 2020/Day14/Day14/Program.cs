using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day14
{
    internal class Program
    {
        #region Class Methods

        public static void Main(string[] args)
        {
            var fileLines = File.ReadAllLines("PuzzleInput.txt");

            var memory = new Dictionary<long, long>();

            var currentMask = string.Empty;

            foreach (var line in fileLines)
            {
                if (line.StartsWith("mask"))
                {
                    // Update current mask
                    currentMask = line.Split(" = ")[1];
                }
                else
                {
                    // Write to memory
                    var splitLine = line.Split(" = ");

                    var memoryValue = long.Parse(splitLine[1]);

                    var memoryLocation = long.Parse(splitLine[0]
                                                    .Substring(splitLine[0]
                                                                   .IndexOf('[')
                                                               + 1)
                                                    .TrimEnd(']'));

                    var memoryBitValue = Convert.ToString(memoryValue,
                                                          2)
                                                .PadLeft(currentMask.Length,
                                                         '0');

                    var maskedValue = Convert.ToInt64(string.Concat(memoryBitValue.Select((memChar,
                                                                                           idx) =>
                                                                                          {
                                                                                              var maskAtIndex = currentMask[idx];

                                                                                              return maskAtIndex == 'X'
                                                                                                         ? memChar
                                                                                                         : maskAtIndex;
                                                                                          })),
                                                      2);

                    memory[memoryLocation] = maskedValue;
                }
            }

            Console.WriteLine($"Memory sum = {memory.Sum(x => x.Value)}.");

            //PT 2

            memory = new Dictionary<long, long>();

            currentMask = string.Empty;

            foreach (var line in fileLines)
            {
                if (line.StartsWith("mask"))
                {
                    // Update current mask
                    currentMask = line.Split(" = ")[1];
                }
                else
                {
                    // Write to memory
                    var splitLine = line.Split(" = ");

                    var memoryValue = long.Parse(splitLine[1]);

                    var memoryLocation = long.Parse(splitLine[0]
                                                    .Substring(splitLine[0]
                                                                   .IndexOf('[')
                                                               + 1)
                                                    .TrimEnd(']'));

                    var memoryLocBitValue = Convert.ToString(memoryLocation,
                                                             2)
                                                   .PadLeft(currentMask.Length,
                                                            '0');

                    var maskedLocations = memoryLocBitValue.Select((memChar,
                                                                    idx) =>
                                                                   {
                                                                       var maskAtIndex = currentMask[idx];

                                                                       if (maskAtIndex == '0')
                                                                       {
                                                                           return new[]
                                                                                  {
                                                                                      memChar
                                                                                  };
                                                                       }

                                                                       if (maskAtIndex == '1')
                                                                       {
                                                                           return new[]
                                                                                  {
                                                                                      maskAtIndex
                                                                                  };
                                                                       }

                                                                       if (maskAtIndex == 'X')
                                                                       {
                                                                           return new[]
                                                                                  {
                                                                                      '0',
                                                                                      '1'
                                                                                  };
                                                                       }

                                                                       throw new InvalidOperationException($"Invalid mask");
                                                                   });

                    var memoryLocationsToWrite = maskedLocations.Aggregate(Enumerable.Empty<IEnumerable<char>>(),
                                                                           (seed,
                                                                            nextCharArray) =>
                                                                           {
                                                                               // Need the ToList here to prevent multiple enumaration issues
                                                                               var aggregatedOptions = AggregateStrings(seed,
                                                                                                                        nextCharArray)
                                                                                   .ToList();

                                                                               return aggregatedOptions;
                                                                           })
                                                                .Select(x => Convert.ToInt64(string.Concat(x),
                                                                                             2))
                                                                .ToList();

                    foreach (var loc in memoryLocationsToWrite)
                    {
                        memory[loc] = memoryValue;
                    }
                }
            }

            Console.WriteLine($"PT 2 Memory sum = {memory.Sum(x => x.Value)}.");
        }

        private static IEnumerable<char[]> AggregateStrings(IEnumerable<IEnumerable<char>> seed,
                                                                char[] characters)
        {
            // Each value of "seed" is a possible combination of characters we could have made up to this point
            // And characters are the characters we can use at this index
            if (!seed.Any())
            {
                // When nothing in the seed yet we need to start it off with our characters
                foreach (var possibleChar in characters)
                {
                    yield return new[]
                                 {
                                     possibleChar
                                 };
                }
            }
            else
            {
                // Otherwise for each possible string so far
                foreach (var stringStart in seed)
                {
                    // Return each possible character we could add to that string
                    foreach (var possibleChar in characters)
                    {
                        yield return stringStart.Append(possibleChar)
                                                .ToArray();
                    }
                }
            }
        }

        #endregion
    }
}