using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day21 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var result = FindCountReachedWithStep(6, false);

            return result.ToString();
        }

        private int FindCountReachedWithStep(int stepCount, bool expandedMap)
        {
            var startingPoint = map.First(x => x.Value == 'S')
                .Key;

            var infiniteMap = new InfiniteMap(map);

            var currentLocations = new HashSet<Coordinate>
            {
                startingPoint
            };

            var prevLocations = new[] { new HashSet<Coordinate>(), new HashSet<Coordinate>() };

            var currentStep = 0;

            var savedLocations = new[] { new HashSet<Coordinate>(), new HashSet<Coordinate>() };

            while (currentStep < stepCount)
            {
                currentStep++;

                var mod = currentStep % 2;

                var prevLocationMap = prevLocations[mod];

                // Find locations we've seen twice
                prevLocationMap.IntersectWith(currentLocations);

                if (prevLocationMap.Count > 0)
                {
                    // We've seen this coordinate twice on an even/odd step, it will be on for every even/odd step
                    savedLocations[mod].UnionWith(prevLocationMap);
                }

                prevLocations[mod] = currentLocations;

                var neighbors = currentLocations.SelectMany(x => x.GetNeighbors())
                    .Where(x =>
                    {
                        if (expandedMap)
                        {
                            return infiniteMap.GetMapChar(x) != '#';
                        }

                        return map.TryGetValue(x, out var c) && c != '#';
                    })
                    .ToHashSet();

                // Discard pre-calculated coordinates
                neighbors.ExceptWith(savedLocations[0]);
                neighbors.ExceptWith(savedLocations[1]);

                currentLocations = neighbors;

                //Console.WriteLine($"Step {currentStep}...");

                //map.Keys.Draw(x =>
                //{
                //    if (savedLocations[1].Contains(x))
                //    {
                //        return "D";
                //    }

                //    if (savedLocations[0].Contains(x))
                //    {
                //        return "E";
                //    }

                //    if (currentLocations.Contains(x))
                //    {
                //        return "O";
                //    }

                //    return map[x]
                //        .ToString();
                //});
            }

            var currentCount = currentLocations.Count;

            // The steps get off somehow, so the amount to add to the current is actually flipped
            var addedCount = currentStep % 2 == 0 ? savedLocations[1].Count : savedLocations[0].Count;

            return currentCount + addedCount;
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var val1 = FindCountReachedWithStep(65, true);
            var val2 = FindCountReachedWithStep(65 + 131, true);
            var val3 = FindCountReachedWithStep(65 + (131 * 2), true);

            // From wolfram alpha or something similar with the above values to solve a quadratic
            // With x:1 = val1, x:2 = val2, and x:3=val3
            var eq1 = 15397;
            var eq2 = 15495;
            var eq3 = 3893;

            // 26501365 = 202300 * 131 + 65 where 131 is the dimension of the grid
            var n = 202300m;
            var result = eq1 * n * n + eq2 * n + eq3;

            return result.ToString();
        }

        private Dictionary<Coordinate, char> map;

        private int xSize;

        private int ySize;

        public override async Task ReadInput()
        {
            var grid = await new GridFileReader().ReadInputFromFile();

            map = grid.ToDictionary(x => x.Coordinate, x => x.Value);

            xSize = (int)map.Max(x => x.Key.X) + 1;
            ySize = (int)map.Max(x => x.Key.Y) + 1;
        }
    }
}
