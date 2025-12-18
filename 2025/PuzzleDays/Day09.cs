using Helpers.Drawing;
using Helpers.Extensions;
using Helpers.Maps;
using Helpers.Structure;
using System.Globalization;
using Helpers.Interfaces;

namespace PuzzleDays
{
    public class Day09 : SingleExecutionPuzzle<Day09.State>, IVisualize2d
    {
        public override PuzzleInfo Info => new(2025, 9, "Movie Theater");

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var max = 0m;

            foreach (var combo in InitialState.RedTiles.Combinations(2))
            {
                var cornerOne = combo[0];
                var cornerTwo = combo[1];

                // +1 since the corners are included
                var width = Math.Abs(cornerOne.X - cornerTwo.X) + 1;
                var height = Math.Abs(cornerOne.Y - cornerTwo.Y) + 1;

                var area = width * height;
                max = Math.Max(max, area);
            }

            return max.ToString(CultureInfo.InvariantCulture);
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            var max = 0m;

            var xCoordinateMap = new Dictionary<int, decimal>();
            var yCoordinateMap = new Dictionary<int, decimal>();

            // Compress the grid, this cuts down on the size of area to check
            var distinctXInOrder = InitialState.RedTiles.Select(x => x.X)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            var distinctYInOrder = InitialState.RedTiles.Select(x => x.Y)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var mappedX = 0;
            foreach (var x in distinctXInOrder)
            {
                xCoordinateMap[mappedX] = x;
                mappedX++;
            }

            var mappedY = 0;
            foreach (var y in distinctYInOrder)
            {
                yCoordinateMap[mappedY] = y;
                mappedY++;
            }

            var xCoordinateInvert = xCoordinateMap.ToDictionary(x => x.Value, x => x.Key);
            var yCoordinateInvert = yCoordinateMap.ToDictionary(x => x.Value, x => x.Key);

            // Now create a converted list of red tiles
            var convertedRedTiles = InitialState.RedTiles
                .Select(x => new Coordinate(xCoordinateInvert[x.X], yCoordinateInvert[x.Y]))
                .ToList();

            // Now figure out all the green tiles in the converted space
            var greenTiles = new HashSet<Coordinate>();
            // Start with the border
            for (var i = 0; i < convertedRedTiles.Count - 1; i++)
            {
                var line = new Line
                {
                    Start = convertedRedTiles[i],
                    End = convertedRedTiles[i + 1]
                };

                greenTiles.UnionWith(line.GetAllCoordinates());
            }

            var finalLine = new Line
            {
                Start = convertedRedTiles[^1],
                End = convertedRedTiles[0]
            };

            greenTiles.UnionWith(finalLine.GetAllCoordinates());

            var maxX = greenTiles.Max(x => x.X);

            // Now get the inner tiles
            Coordinate innerPointStart = null!;
            var foundPoint = false;
            foreach (var possibleDirection in
                     new List<Direction>
                         { Direction.SouthEast, Direction.SouthWest, Direction.NorthEast, Direction.NorthWest })
            {
                innerPointStart = convertedRedTiles[0]
                    .GetDirection(possibleDirection);

                // Test if it's in the polygon
                var crossings = 0;
                var current = innerPointStart;

                // The + 10 was just to get past the very edge of the bounding box
                while (current.X < maxX + 10)
                {
                    // Keep moving right from the possible inner point
                    current = current.GetDirection(Direction.East);

                    // And track any time it crosses a border
                    if (greenTiles.Contains(current))
                    {
                        crossings++;
                    }
                }

                if (crossings % 2 != 0)
                {
                    // We crossed the borders an odd amount of times, so we started from inside
                    foundPoint = true;
                    break;
                }
            }

            if (!foundPoint)
            {
                throw new InvalidOperationException("Didn't find valid inner point");
            }

            var validTiles = greenTiles.Concat(convertedRedTiles)
                .ToHashSet();

            // Now flood until we find all inner points
            var queue = new Queue<Coordinate>();
            queue.Enqueue(innerPointStart);

            var visited = new HashSet<Coordinate> { innerPointStart };

            while (queue.Count > 0)
            {
                var next = queue.Dequeue();

                var neighbors = next.GetNeighbors()
                    .Where(x => !visited.Contains(x) && !validTiles.Contains(x))
                    .ToList();

                foreach (var neighbor in neighbors)
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }

            validTiles.UnionWith(visited);

            foreach (var combo in convertedRedTiles.Combinations(2))
            {
                // Start like normal
                var cornerOne = combo[0];
                var cornerTwo = combo[1];
                var cornerThree = new Coordinate(cornerOne.X, cornerTwo.Y);
                var cornerFour = new Coordinate(cornerTwo.X, cornerOne.Y);

                // Get the borders of this rectangle
                var rectangleLines = new List<Line>
                {
                    new()
                    {
                        Start = cornerOne,
                        End = cornerThree
                    },
                    new()
                    {
                        Start = cornerOne,
                        End = cornerFour
                    },
                    new()
                    {
                        Start = cornerTwo,
                        End = cornerThree
                    },
                    new()
                    {
                        Start = cornerTwo,
                        End = cornerFour
                    }
                };

                // If any of the lines on the border aren't in the valid tile list, it's not valid
                if (rectangleLines.Any(l => l.GetAllCoordinates()
                                           .Any(c => !validTiles.Contains(c))))
                {
                    continue;
                }

                // Convert back to the original coordinate values before calculating area
                var cornerOneOriginal = new Coordinate(xCoordinateMap[(int)cornerOne.X], yCoordinateMap[(int)cornerOne.Y]);
                var cornerTwoOriginal = new Coordinate(xCoordinateMap[(int)cornerTwo.X], yCoordinateMap[(int)cornerTwo.Y]);

                // +1 since the corners are included
                var width = Math.Abs(cornerOneOriginal.X - cornerTwoOriginal.X) + 1;
                var height = Math.Abs(cornerOneOriginal.Y - cornerTwoOriginal.Y) + 1;

                var area = width * height;
                max = Math.Max(max, area);
            }

            return max.ToString(CultureInfo.InvariantCulture);
        }

        protected override async Task<State> LoadInputState(string puzzleInput, PuzzleInputType inputType)
        {
            var redTiles = puzzleInput.Split('\n')
                .Select(x =>
                {
                    var split = x.Split(',');
                    return new Coordinate(int.Parse(split[0]), int.Parse(split[1]));
                });

            return new State
            {
                RedTiles = redTiles.ToList()
            };
        }

        public class State
        {
            public required List<Coordinate> RedTiles { get; set; }
        }

        public class Line
        {
            public required Coordinate Start { get; set; }
            public required Coordinate End { get; set; }

            public HashSet<Coordinate> GetAllCoordinates()
            {
                if (Start == End)
                {
                    return [Start];
                }

                // We know in our case the line is never diagonal
                // Figure out the direction to walk
                var direction = GetLineDirection();

                var current = Start.GetDirection(direction);
                var result = new HashSet<Coordinate>
                {
                    Start,
                    End
                };

                while (current != End)
                {
                    result.Add(current);
                    current = current.GetDirection(direction);
                }

                return result;
            }

            public Direction GetLineDirection()
            {
                var direction = Start.X == End.X ? Start.Y > End.Y ? Direction.South : Direction.North
                    : Start.X > End.X ? Direction.West : Direction.East;
                return direction;
            }
        }

        private DrawableCoordinate[] toDraw = [];

        public DrawableCoordinate[] GetCoordinates()
        {
            return toDraw;
        }
    }
}
