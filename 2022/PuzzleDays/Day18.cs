using Helpers;
using Helpers.Extensions;
using Helpers.Maps._3D;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day18 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var exposedSides = 0;

            var coordHash = coordinates.ToHashSet();

            foreach (var coordinate in coordinates)
            {
                var baseExposedSides = 6;

                var coveredSides = coordinate.GetNeighbors().Count(x => coordHash.Contains(x));

                exposedSides += (baseExposedSides - coveredSides);
            }

            return exposedSides.ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        public override async Task ReadInput()
        {
            coordinates = await new CoordinateReader().ReadInputFromFile();
        }

        private List<Coordinate3d> coordinates;
    }
}

class CoordinateReader : FileReader<Coordinate3d>
{
    protected override Coordinate3d ProcessLineOfFile(string line)
    {
        var split = line.Split(',');

        return new Coordinate3d(int.Parse(split[0]),
                                int.Parse(split[1]),
                                int.Parse(split[2]));
    }
}