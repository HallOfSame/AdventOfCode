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
    public class Day10 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            // DrawPipes();

            var queueToCheck = new Queue<(Coordinate current, List<Coordinate> path)>();

            // Find pipe sections that connect to the starting point
            var startingPoints = startingPoint.GetNeighbors()
                .Where((coord, idx) =>
                {
                    var direction = (Direction)idx;

                    if (!map.TryGetValue(coord, out var pipeSection))
                    {
                        return false;
                    }

                    if (direction == Direction.North)
                    {
                        return pipeSection.Direction is PipeDirection.NorthSouth or PipeDirection.SouthEast
                            or PipeDirection.SouthWest;
                    }

                    if (direction == Direction.South)
                    {
                        return pipeSection.Direction is PipeDirection.NorthSouth or PipeDirection.NorthEast
                            or PipeDirection.NorthWest;
                    }

                    if (direction == Direction.East)
                    {
                        return pipeSection.Direction is PipeDirection.NorthWest or PipeDirection.EastWest
                            or PipeDirection.SouthWest;
                    }

                    if (direction == Direction.West)
                    {
                        return pipeSection.Direction is PipeDirection.EastWest or PipeDirection.NorthEast
                            or PipeDirection.SouthEast;
                    }

                    throw new Exception("Unexpected neighbor direction");
                })
                .ToList();

            // Add each of those to the queue with the beginning path
            startingPoints.ForEach(x => queueToCheck.Enqueue((x, new List<Coordinate> { startingPoint })));
            
            List<Coordinate> loopPath = null;

            while (queueToCheck.Any())
            {
                var (current, path) = queueToCheck.Dequeue();

                var prev = path.Last();
                
                var nextPipeSection = map[current];

                // The neighbor we are moving towards it the one that isn't our prev spot
                var neighbor = nextPipeSection
                    .GetNeighborCoordinates()
                    .First(x => x != prev);

                if (neighbor == startingPoint)
                {
                    // Found the start again, so we completed the loop
                    path.Add(current);
                    loopPath = path;
                    break;
                }

                // Otherwise if a valid pipe exists at the next step, add it to the queue and the path
                if (map.ContainsKey(neighbor))
                {
                    path.Add(current);
                    queueToCheck.Enqueue((neighbor, path));
                }
            }

            return (loopPath.Count / 2).ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        private void DrawPipes()
        {
            new List<Coordinate> { startingPoint }.Concat(map.Keys)
                .Draw((c) =>
                {
                    if (c == startingPoint)
                    {
                        return "S";
                    }

                    var pipe = map[c];

                    return pipe.Direction switch
                    {
                        PipeDirection.NorthSouth => "|",
                        PipeDirection.EastWest => "-",
                        PipeDirection.NorthEast => "L",
                        PipeDirection.NorthWest => "J",
                        PipeDirection.SouthWest => "7",
                        PipeDirection.SouthEast => "F",
                        _ => throw new Exception("Missing direction"),
                    };
                }, forceOrigin: true);
        }

        public override async Task ReadInput()
        {
            var strings = await new StringFileReader().ReadInputFromFile();

            // Reverse to make the origin the bottom left
            strings.Reverse();

            map = new Dictionary<Coordinate, PipeSection>();

            for (var y = 0; y < strings.Count; y++)
            {
                for (var x = 0;
                     x < strings[0]
                         .Length;
                     x++)
                {
                    if (strings[y][x] == '.')
                    {
                        continue;
                    }

                    if (strings[y][x] == 'S')
                    {
                        startingPoint = new Coordinate(x, y);
                        continue;
                    }

                    var pipeType = strings[y][x] switch
                    {
                        '|' => PipeDirection.NorthSouth,
                        '-' => PipeDirection.EastWest,
                        'L' => PipeDirection.NorthEast,
                        'J' => PipeDirection.NorthWest,
                        '7' => PipeDirection.SouthWest,
                        'F' => PipeDirection.SouthEast,
                        _ => throw new Exception("Unexpected character")
                    };

                    var coordinate = new Coordinate(x, y);

                    var newPipe = new PipeSection(coordinate)
                    {
                        Direction = pipeType,
                    };

                    map.Add(coordinate, newPipe);
                }
            }
        }

        private Coordinate startingPoint;

        private Dictionary<Coordinate, PipeSection> map;

        class PipeSection : ObjectWithCoordinateEquality
        {
            public PipeDirection Direction { get; set; }

            public Coordinate[] GetNeighborCoordinates()
            {
                var neighbors = new Coordinate[2];

                if (Direction is PipeDirection.NorthSouth or PipeDirection.NorthEast or PipeDirection.NorthWest)
                {
                    // Add north coordinate
                    neighbors[0] = new Coordinate(this.Coordinate.X, this.Coordinate.Y + 1);
                }
                
                if (Direction is PipeDirection.NorthSouth or PipeDirection.SouthEast or PipeDirection.SouthWest)
                {
                    // Add south coordinate
                    neighbors[Direction == PipeDirection.NorthSouth ? 1 : 0] = new Coordinate(this.Coordinate.X, this.Coordinate.Y - 1);
                }

                if (Direction is PipeDirection.EastWest or PipeDirection.NorthEast or PipeDirection.SouthEast)
                {
                    // Add East
                    neighbors[Direction == PipeDirection.EastWest ? 0 : 1] = new Coordinate(this.Coordinate.X + 1, this.Coordinate.Y);
                }

                if (Direction is PipeDirection.EastWest or PipeDirection.NorthWest or PipeDirection.SouthWest)
                {
                    // Add West
                    neighbors[1] = new Coordinate(this.Coordinate.X - 1, this.Coordinate.Y);
                }

                return neighbors;
            }

            public PipeSection(Coordinate coordinate) : base(coordinate)
            {

            }
        }

        enum PipeDirection
        {
            NorthSouth,
            EastWest,
            NorthEast,
            NorthWest,
            SouthWest,
            SouthEast,
        }
    }
}
