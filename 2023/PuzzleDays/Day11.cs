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
    public class Day11 : ProblemBase
    {
        private List<int> rowIndexesToExpand;

        private List<int> colIndexesToExpand;

        protected override async Task<string> SolvePartOneInternal()
        {
            rowIndexesToExpand = rawFile.Select((x,
                                                 idx) => (str: x, idx))
                                        .Where(x => x.str.All(ch => ch == '.'))
                                        .Select(x => x.idx)
                                        .ToList();

            var widthOfOriginalMap = rawFile[0]
                .Length;

            colIndexesToExpand = Enumerable.Range(0,
                                                  widthOfOriginalMap)
                                           .Where(idx => rawFile.All(x => x[idx] == '.'))
                                           .ToList();

            var updatedFile = rawFile.ToList();

            var addedRowsSoFar = 0;

            rowIndexesToExpand.ForEach(idx =>
                                       {
                                           updatedFile.Insert(idx + addedRowsSoFar,
                                                              new string(Enumerable.Repeat('.',
                                                                                           widthOfOriginalMap)
                                                                                   .ToArray()));
                                           addedRowsSoFar++;
                                       });

            var addedColumnsSoFar = 0;

            colIndexesToExpand.ForEach(col =>
                                       {
                                           updatedFile = updatedFile.Select(str => str.Insert(col + addedColumnsSoFar,
                                                                                              "."))
                                                                    .ToList();
                                           addedColumnsSoFar++;
                                       });

            var galaxies = new List<Coordinate>();

            for (var y = 0; y < updatedFile.Count; y++)
            {
                var lineOfFile = updatedFile[y];

                for (var x = 0;
                     x
                     < updatedFile[0]
                         .Length;
                     x++)
                {
                    if (lineOfFile[x] == '#')
                    {
                        galaxies.Add(new Coordinate(x,
                                                    y));
                    }
                }
            }

            var galaxiCombos = galaxies.Combinations(2);

            var distanceSum = galaxiCombos.Select(x => CoordinateHelper.ManhattanDistance(x.First(),
                                                                                          x.Last()))
                                          .Sum();

            return distanceSum.ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var levelOfExpansion = 1000000;

            // Find galaxies in original file
            var galaxies = new List<Coordinate>();

            for (var y = 0; y < rawFile.Count; y++)
            {
                var lineOfFile = rawFile[y];

                for (var x = 0;
                     x
                     < rawFile[0]
                         .Length;
                     x++)
                {
                    if (lineOfFile[x] == '#')
                    {
                        galaxies.Add(new Coordinate(x,
                                                    y));
                    }
                }
            }

            var galaxiCombos = galaxies.Combinations(2);

            var runningSum = 0L;

            foreach (var combo in galaxiCombos)
            {
                var c1 = combo.First();
                var c2 = combo.Last();

                var minCol = Math.Min(c1.X,
                                      c2.X);
                var maxCol = Math.Max(c1.X,
                                      c2.X);
                var minRow = Math.Min(c1.Y,
                                      c2.Y);
                var maxRow = Math.Max(c1.Y,
                                      c2.Y);

                var expandedSectionsPassed = rowIndexesToExpand.Count(idx => idx >= minRow && idx <= maxRow) + colIndexesToExpand.Count(idx => idx >= minCol && idx <= maxCol);

                var initialDistance = CoordinateHelper.ManhattanDistance(c1,
                                                                         c2);

                var updatedDistance = (initialDistance - expandedSectionsPassed) + (expandedSectionsPassed * levelOfExpansion);

                runningSum += updatedDistance;
            }

            return runningSum.ToString();
        }

        public override async Task ReadInput()
        {
            var strings = await new StringFileReader().ReadInputFromFile();

            rawFile = strings;
            rawFile.Reverse();
        }

        private List<string> rawFile;
    }
}
