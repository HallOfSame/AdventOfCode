using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day16 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var maxY = coordinateMap.Keys.Max(x => x.Y);

            return GetEnergizeCount(new Beam(new Coordinate(-1, maxY), Direction.East))
                .ToString();
        }

        private int GetEnergizeCount(Beam startingBeam)
        {
            var beams = new List<Beam>
            {
                startingBeam
            };

            var visited = new HashSet<Coordinate>();

            var handled = new HashSet<(Coordinate, Direction)>();

            while (true)
            {
                var currentBeams = beams.ToList();
                beams.Clear();

                foreach (var beam in currentBeams)
                {
                    if (beam.CurrentLocation is null)
                    {
                        visited.UnionWith(beam.VisitedCoordinates);
                        continue;
                    }

                    if (handled.Contains((beam.CurrentLocation, beam.CurrentDirection)))
                    {
                        visited.UnionWith(beam.VisitedCoordinates);
                        // Another beam has been here, so ignore it
                        continue;
                    }

                    handled.Add((beam.CurrentLocation, beam.CurrentDirection));

                    var updatedBeam = ProcessMove(beam);

                    beams.AddRange(updatedBeam);
                }

                // // Magic sequence that helps clear the console
                //Console.Clear();
                //Console.WriteLine("\x1b[3J");

                //coordinateMap.Keys.Draw(x =>
                //{
                //    var beamHere = beams.FirstOrDefault(b => b.CurrentLocation == x);

                //    if (beamHere is not null)
                //    {
                //        return beamHere.CurrentDirection switch
                //        {
                //            Direction.East => ">",
                //            Direction.North => "^",
                //            Direction.South => "v",
                //            Direction.West => "<",
                //            _ => throw new Exception()
                //        };
                //    }

                //    return coordinateMap[x]
                //        .ToString();
                //});

                // Keep going until we have processed all movement
                if (beams.Any())
                {
                    // Thread.Sleep(150);
                    continue;
                }

                // -1 because our starting x = -1 coordinate isn't real
                var energizedCount = visited.Count - 1;

                //coordinateMap.Keys.Draw(x =>
                //{
                //    if (visited.Contains(x))
                //    {
                //        return "#";
                //    }

                //    return coordinateMap[x]
                //        .ToString();
                //});

                return energizedCount;
            }
        }

        private List<Beam> ProcessMove(Beam beam)
        {
            var returnBeams = new List<Beam>(2)
            {
                beam
            };

            var nextCoordinate = beam.CurrentLocation!.GetDirection(beam.CurrentDirection);

            if (!coordinateMap.TryGetValue(nextCoordinate, out var ch))
            {
                // If it is off the grid, return null
                beam.CurrentLocation = null;
                return returnBeams;
            }

            var beamDirection = beam.CurrentDirection;
            beam.CurrentLocation = nextCoordinate;

            if (ch == '.')
            {
                // Empty space
                return returnBeams;
            }

            // Handle pointy side of splitter
            if (beamDirection is Direction.East or Direction.West && ch == '-')
            {
                // Essentially empty space
                return returnBeams;
            }

            if (beamDirection is Direction.South or Direction.North && ch == '|')
            {
                // Essentially empty space
                return returnBeams;
            }

            // Handle flat side of splitters
            if (beamDirection is Direction.East or Direction.West && ch == '|')
            {
                var locOne = nextCoordinate.GetDirection(Direction.North);
                
                var locTwo = nextCoordinate.GetDirection(Direction.South);

                // If only one of the two exists, try and keep the existing beam
                if (!coordinateMap.ContainsKey(locOne))
                {
                    beam.CurrentDirection = Direction.South;
                }
                else
                {
                    beam.CurrentDirection = Direction.North;

                    if (coordinateMap.ContainsKey(locTwo))
                    {
                        var newBeam = new Beam(nextCoordinate, Direction.South);
                        returnBeams.Add(newBeam);
                    }
                }
                
                return returnBeams;
            }

            if (beamDirection is Direction.North or Direction.South && ch == '-')
            {
                var locOne = nextCoordinate.GetDirection(Direction.East);
                
                var locTwo = nextCoordinate.GetDirection(Direction.West);

                // If only one of the two exists, try and keep the existing beam
                if (!coordinateMap.ContainsKey(locOne))
                {
                    beam.CurrentDirection = Direction.West;
                }
                else
                {
                    beam.CurrentDirection = Direction.East;

                    if (coordinateMap.ContainsKey(locTwo))
                    {
                        var newBeam = new Beam(nextCoordinate, Direction.West);
                        returnBeams.Add(newBeam);
                    }
                }
                
                return returnBeams;
            }

            // Handle mirrors
            if (ch == '/')
            {
                var newDirection = beamDirection switch
                {
                    Direction.South => Direction.West,
                    Direction.North => Direction.East,
                    Direction.East => Direction.North,
                    Direction.West => Direction.South,
                    _ => throw new NotImplementedException(),
                };

                beam.CurrentDirection = newDirection;
                return returnBeams;
            }

            if (ch == '\\')
            {
                var newDirection = beamDirection switch
                {
                    Direction.South => Direction.East,
                    Direction.North => Direction.West,
                    Direction.East => Direction.South,
                    Direction.West => Direction.North,
                    _ => throw new NotImplementedException(),
                };

                beam.CurrentDirection = newDirection;
                return returnBeams;
            }

            throw new Exception($"Unexpected character {ch}");
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var maxValue = 0;
            var maxX = coordinateMap.Max(x => x.Key.X);
            var maxY = coordinateMap.Max(x => x.Key.Y);

            // Top row
            for (var x = 0; x <= maxX; x++)
            {
                var startBeam = new Beam(new Coordinate(x, maxY + 1), Direction.South);

                var energizeCount = GetEnergizeCount(startBeam);

                maxValue = Math.Max(energizeCount, maxValue);
            }

            // Bottom row
            for (var x = 0; x <= maxX; x++)
            {
                var startBeam = new Beam(new Coordinate(x, -1), Direction.North);

                var energizeCount = GetEnergizeCount(startBeam);

                maxValue = Math.Max(energizeCount, maxValue);
            }

            // Left side
            for (var y = 0; y <= maxY; y++)
            {
                var startBeam = new Beam(new Coordinate(-1, y), Direction.East);

                var energizeCount = GetEnergizeCount(startBeam);

                maxValue = Math.Max(energizeCount, maxValue);
            }

            // Right side
            for (var y = 0; y <= maxY; y++)
            {
                var startBeam = new Beam(new Coordinate(maxX + 1, y), Direction.West);

                var energizeCount = GetEnergizeCount(startBeam);

                maxValue = Math.Max(energizeCount, maxValue);
            }

            return maxValue.ToString();
        }

        private Dictionary<Coordinate, char> coordinateMap;

        public override async Task ReadInput()
        {
            coordinateMap =
                (await new GridFileReader().ReadInputFromFile()).ToDictionary(x => x.Coordinate, x => x.Value);
        }

        class Beam
        {
            public Beam(Coordinate loc, Direction d)
            {
                VisitedCoordinates = new HashSet<Coordinate>
                {
                    loc
                };
                CurrentLocation = loc;
                CurrentDirection = d;
            }

            private Coordinate? currLoc;

            public Coordinate? CurrentLocation
            {
                get => currLoc;
                set
                {
                    if (value is not null)
                    {
                        VisitedCoordinates.Add(value);
                    }

                    currLoc = value;
                }
            }

            public Direction CurrentDirection { get; set; }

            public HashSet<Coordinate> VisitedCoordinates { get; }
        }
    }
}
