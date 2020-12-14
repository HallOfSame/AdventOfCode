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
        }

        #endregion
    }
}