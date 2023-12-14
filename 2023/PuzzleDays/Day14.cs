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
    public class Day14 : ProblemBase
    {
        private const char CircularRock = 'O';
        private const char CubeRock = '#';
        private const char EmptySpace = '.';

        protected override async Task<string> SolvePartOneInternal()
        {
            var updatedCoordinates = RunCycle(Direction.North, coordinates);

            var result = CalculateSupportLoad(updatedCoordinates);

            return result.ToString();
        }

        private List<CoordinateWithCharacter> RunCycle(Direction direction,
                                                       List<CoordinateWithCharacter> inputCoordinates)
        {
            Func<Coordinate, int> maxCoordinateSelector = direction switch
            {
                Direction.East => c => c.X,
                Direction.West => c => 0 - c.X,
                Direction.North => c => c.Y,
                Direction.South => c => 0 - c.Y,
                _ => throw new Exception("Not a cardinal direction")
            };

            Func<Coordinate, int, bool> matchingCoordinateSelector = direction switch
            {
                Direction.East or Direction.West => (c, x) => c.X == x,
                Direction.North or Direction.South => (c, y) => c.Y == y,
                _ => throw new Exception("Not a cardinal direction")
            };

            var coordinateCopy = inputCoordinates.ToList();
            var coordinateMap = coordinateCopy.ToDictionary(x => x.Coordinate, x => x);

            var maxDirection = coordinateCopy.Max(x => maxCoordinateSelector(x.Coordinate));

            for (var j = maxDirection; j >= 0; j--)
            {
                var circleRocksToProcess = coordinateCopy.Where(coord => matchingCoordinateSelector(coord.Coordinate, j) && coord.Value == CircularRock);

                foreach (var circleRock in circleRocksToProcess)
                {
                    var nextCoordinate = circleRock.Coordinate.GetDirection(direction);

                    while (coordinateMap.TryGetValue(nextCoordinate, out var next) && next.Value == EmptySpace)
                    {
                        var originalCoordinate = circleRock.Coordinate;
                        circleRock.Coordinate = nextCoordinate;
                        coordinateMap[nextCoordinate] = circleRock;
                        coordinateMap[originalCoordinate] = new CoordinateWithCharacter(originalCoordinate)
                        {
                            Value = EmptySpace
                        };

                        nextCoordinate = circleRock.Coordinate.GetDirection(direction);
                    }
                }
            }

            return coordinateCopy;
        }

        private int CalculateSupportLoad(List<CoordinateWithCharacter> coordinates)
        {
            return coordinates.Where(x => x.Value == CircularRock)
                .Select(x => x.Coordinate.Y + 1)
                .Sum();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        private List<CoordinateWithCharacter> coordinates;

        public override async Task ReadInput()
        {
            coordinates = await new GridFileReader().ReadInputFromFile();
        }
    }
}
