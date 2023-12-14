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
            var updatedCoordinates = coordinates.ToList();
            var coordinateMap = updatedCoordinates.ToDictionary(x => x.Coordinate, x => x);

            var maxY = updatedCoordinates.Max(coord => coord.Coordinate.Y);

            // coordinateMap.Keys.Draw(x => coordinateMap[x].Value.ToString(), "o");

            // Starting as north as possible, roll circular rocks north until blocked
            for (var y = maxY; y >= 0; y--)
            {
                var circleRocksOnThisRow = updatedCoordinates.Where(coord => coord.Coordinate.Y == y && coord.Value == CircularRock);

                foreach (var circleRock in circleRocksOnThisRow)
                {
                    var northCoordinate = new Coordinate(circleRock.Coordinate.X, circleRock.Coordinate.Y + 1);

                    while (coordinateMap.TryGetValue(northCoordinate, out var north) && north.Value == EmptySpace)
                    {
                        circleRock.Coordinate.Y += 1;
                        coordinateMap[northCoordinate] = circleRock;
                        coordinateMap[new Coordinate(circleRock.Coordinate.X, circleRock.Coordinate.Y - 1)] = new CoordinateWithCharacter(new Coordinate(circleRock.Coordinate.X, circleRock.Coordinate.Y - 1))
                        {
                            Value = EmptySpace
                        };

                        northCoordinate = new Coordinate(circleRock.Coordinate.X, circleRock.Coordinate.Y + 1);
                    }
                }
            }

            // coordinateMap.Keys.Draw(x => coordinateMap[x].Value.ToString(), "o");

            var result = updatedCoordinates.Where(x => x.Value == CircularRock)
                .Select(x => x.Coordinate.Y + 1)
                .Sum();

            return result.ToString();
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
