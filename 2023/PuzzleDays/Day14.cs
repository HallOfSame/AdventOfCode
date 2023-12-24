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
            var updatedCoordinates = RunCycle(Direction.North, coordinates, out _);

            var result = CalculateSupportLoad(updatedCoordinates);

            return result.ToString();
        }

        private List<CoordinateWithCharacter> RunCycle(Direction direction,
                                                       List<CoordinateWithCharacter> inputCoordinates,
                                                       out int moveCount)
        {
            moveCount = 0;
            Func<Coordinate, int> maxCoordinateSelector = direction switch
            {
                Direction.East => c => (int)c.X,
                Direction.West => c => (int)Math.Abs(0 - c.X),
                Direction.North => c => (int)c.Y,
                Direction.South => c => (int)Math.Abs(0 - c.Y),
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

            var jStart = direction == Direction.North || direction == Direction.East ? maxDirection : 0;
            var jEnd = jStart == 0 ? maxDirection : 0;
            var jChange = jStart == 0 ? 1 : -1;
            Func<int, bool> jTest = jStart == 0 ? (j) => j <= jEnd : (j) => j >= jEnd;

            for (var j = jStart; jTest(j); j += jChange)
            {
                var circleRocksToProcess = coordinateCopy.Where(coord => matchingCoordinateSelector(coord.Coordinate, j) && coord.Value == CircularRock);

                foreach (var circleRock in circleRocksToProcess)
                {
                    var current = coordinateMap[circleRock.Coordinate];
                    var nextCoordinate = circleRock.Coordinate.GetDirection(direction);

                    while (coordinateMap.TryGetValue(nextCoordinate, out var next) && next.Value == EmptySpace)
                    {
                        moveCount++;
                        current.Value = EmptySpace;
                        coordinateMap[nextCoordinate]
                            .Value = CircularRock;

                        current = coordinateMap[nextCoordinate];
                        nextCoordinate = nextCoordinate.GetDirection(direction);
                    }
                }
            }

            //if (direction == Direction.East)
            //{
            //    coordinateMap.Keys.Draw(x => coordinateMap[x].Value.ToString(), "o");
            //}

            return coordinateCopy;
        }

        private int CalculateSupportLoad(List<CoordinateWithCharacter> coordinates)
        {
            return (int)coordinates.Where(x => x.Value == CircularRock)
                .Select(x => x.Coordinate.Y + 1)
                .Sum();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var updatedCoordinates = coordinates.ToList();

            var directions = new List<Direction>
            {
                Direction.North,
                Direction.West,
                Direction.South,
                Direction.East,
            };

            var prevSeen = new Dictionary<string, int>();

            var remainingCycles = 1000000000;

            for (var cycle = 0; cycle < 1000000000; cycle++)
            {
                remainingCycles -= 1;

                // Off by one, 0 gives the answer before that loop
                // I could probably just move this to after we run the cycle
                if (remainingCycles == -1)
                {
                    break;
                }

                for (var i = 0; i < 4; i++)
                {
                    var direction = directions[i];

                    updatedCoordinates = RunCycle(direction, updatedCoordinates, out var _);
                }

                // Track maps we've seen before
                var hash = string.Join(string.Empty,
                                       updatedCoordinates.OrderByDescending(x => x.Coordinate.Y)
                                           .ThenBy(x => x.Coordinate.X)
                                           .Select(x => x.Value));

                if (prevSeen.TryGetValue(hash, out var currCount))
                {
                    prevSeen[hash] = currCount + 1;
                }
                else
                {
                    prevSeen[hash] = 1;
                }

                // If the current count is 3 and everything in the dictionary is 1 or 3
                // This means we've found the end of a cycle that will loop
                // Using 3 because 1 or 2 won't trigger at the end of a cycle (most parts of the cycle will have count 1)
                if (currCount + 1 == 3 && prevSeen.All(x => x.Value is 1 or 3))
                {
                    // Remove any starter positions that only occur once before the loop begins
                    var loopCycles = 1000000000 - prevSeen.Count(x => x.Value == 1);

                    // From that, calculate how many steps into the loop that cycle 1000000000 will occur
                    var stepsIntoCycleToStop = loopCycles % prevSeen.Count(x => x.Value == 3);

                    // Then update so we run until we hit that point of the loop
                    remainingCycles = stepsIntoCycleToStop;
                }
            }

            var result = CalculateSupportLoad(updatedCoordinates);

            return result.ToString();
        }

        private List<CoordinateWithCharacter> coordinates;

        public override async Task ReadInput()
        {
            coordinates = await new GridFileReader().ReadInputFromFile();
        }
    }
}
