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
        protected override async Task<string> SolvePartOneInternal()
        {
            var rowsToExpand = rawFile.Select((x, idx) => (str: x, idx)).Where(x => x.str.All(ch => ch == '.'))
                .Select(x => x.idx)
                .ToList();

            var widthOfOriginalMap = rawFile[0]
                .Length;

            var columnsToExpand = Enumerable.Range(0, widthOfOriginalMap)
                .Where(idx => rawFile.All(x => x[idx] == '.'))
                .ToList();

            var updatedFile = rawFile.ToList();

            var addedRowsSoFar = 0;

            rowsToExpand.ForEach(idx =>
            {
                updatedFile.Insert(idx + addedRowsSoFar, new string(Enumerable.Repeat('.', widthOfOriginalMap)
                    .ToArray()));
                addedRowsSoFar++;
            });

            var addedColumnsSoFar = 0;

            columnsToExpand.ForEach(col =>
            {
                updatedFile = updatedFile.Select(str => str.Insert(col + addedColumnsSoFar, ".")).ToList();
                addedColumnsSoFar++;
            });

            var galaxies = new List<Coordinate>();

            for (var y = 0; y < updatedFile.Count; y++)
            {
                var lineOfFile = updatedFile[updatedFile.Count - 1 - y];

                for (var x = 0;
                     x < updatedFile[0]
                         .Length;
                     x++)
                {
                    if (lineOfFile[x] == '#')
                    {
                        galaxies.Add(new Coordinate(x, y));
                    }
                }
            }

            var galaxiCombos = galaxies.Combinations(2);

            var distanceSum = galaxiCombos.Select(x => CoordinateHelper.ManhattanDistance(x.First(), x.Last()))
                .Sum();

            return distanceSum.ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        public override async Task ReadInput()
        {
            var strings = await new StringFileReader().ReadInputFromFile();

            rawFile = strings;
        }

        private List<string> rawFile;
    }
}
