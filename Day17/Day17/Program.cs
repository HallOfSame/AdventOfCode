using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Day17
{
    internal class Program
    {
        #region Class Methods

        private static void Main(string[] args)
        {
            var simTurns = 6;

            var fileLines = File.ReadAllLines("PuzzleInput.txt");

            var initActive = new HashSet<Coordinate>();

            // Have the top left of the input = all 0 coordinates
            var currentX = 0;

            foreach (var line in fileLines)
            {
                var lineArray = line.ToCharArray();

                for (var y = 0; y < lineArray.Length; y++)
                {
                    if (lineArray[y] == '#')
                    {
                        initActive.Add(new Coordinate3d(currentX,
                                                        y,
                                                        0));
                    }
                }

                currentX++;
            }

            var dimension = new Dimension(initActive);

            var stopWatch = new Stopwatch();

            for (var i = 0; i < simTurns; i++)
            {
                stopWatch.Restart();
                Simulator.RunOneTurn(dimension);
                stopWatch.Stop();
                Console.WriteLine($"Turn {i + 1}. Time {stopWatch.ElapsedMilliseconds}ms. Cubes {dimension.ActiveCubeCount}.");
            }

            // Should be 310
            Console.WriteLine($"Number of active cubes after {simTurns} turns. {dimension.ActiveCubeCount}.");
        }

        #endregion
    }
}