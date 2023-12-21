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
            var result = FindCountReachedWithStep(64);

            return result.ToString();
        }

        private int FindCountReachedWithStep(int stepCount)
        {
            var startingPoint = map.First(x => x.Value == 'S').Key;

            var currentLocations = new HashSet<Coordinate>
            {
                startingPoint
            };

            var currentStep = 0;

            while (currentStep < stepCount)
            {
                currentStep++;

                var neighbors = currentLocations.SelectMany(x => x.GetNeighbors())
                    .Where(x => map.TryGetValue(x, out var c) && c != '#')
                    .ToHashSet();

                currentLocations = neighbors;

                //Console.WriteLine($"Step {currentStep}...");
                //map.Keys.Draw(x => currentLocations.Contains(x)
                //                  ? "O"
                //                  : map[x]
                //                      .ToString());
            }

            return currentLocations.Count;
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        private Dictionary<Coordinate, char> map;

        public override async Task ReadInput()
        {
            var grid = await new GridFileReader().ReadInputFromFile();

            map = grid.ToDictionary(x => x.Coordinate, x => x.Value);
        }
    }
}
