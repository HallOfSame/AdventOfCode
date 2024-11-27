using Helpers.Drawing;
using Helpers.FileReaders;
using Helpers.Interfaces;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class TestPuzzleSteps : StepExecutionPuzzle<TestPuzzleSteps.ExecutionState>, IVisualize2d
    {
        public class ExecutionState
        {
            public bool Started { get; set; }
            public required List<Beam> CurrentBeams { get; set; }
            public required HashSet<Coordinate> EnergizedCoordinates { get; set; }
            public required HashSet<CoordinateWithDir> AlreadyProcessed { get; set; }
        }

        public record CoordinateWithDir(Coordinate Coordinate, Direction Direction);

        public override PuzzleInfo Info { get; } = new(2023, 17, "The Floor Will Be Lava (stepped)");

        private Dictionary<Coordinate, char> coordinateMap = [];

        protected override async Task<ExecutionState> LoadInitialState(string puzzleInput)
        {
            coordinateMap = (await new GridFileReader().ReadFromString(puzzleInput)).ToDictionary(x => x.Coordinate, x => x.Value);

            return new ExecutionState
            {
                CurrentBeams = [],
                AlreadyProcessed = [],
                EnergizedCoordinates = [],
                Started = false
            };
        }

        protected override Task<(bool isComplete, string? result)> ExecutePuzzleStepPartOne()
        {
            if (!CurrentState.Started)
            {
                var maxY = coordinateMap.Keys.Max(x => x.Y);

                CurrentState.CurrentBeams =
                [
                    new Beam(new Coordinate(-1, maxY), Direction.East)
                ];
            }

            CurrentState = RunEnergizeStep(CurrentState);

            return Task.FromResult((CurrentState.CurrentBeams.Count == 0, CurrentState.EnergizedCoordinates.Count
                                       .ToString()))!;
        }

        protected override async Task<(bool isComplete, string? result)> ExecutePuzzleStepPartTwo()
        {
            throw new NotImplementedException();
        }

        public DrawableCoordinate[] GetCoordinates()
        {
            var processedMap = CurrentState.AlreadyProcessed.GroupBy(x => x.Coordinate).ToDictionary(x => x.Key, x => x.First().Direction);
            var toDraw = new List<DrawableCoordinate>(coordinateMap.Count);
            foreach (var coordinate in coordinateMap)
            {
                var newCoordinate = new DrawableCoordinate
                {
                    X = coordinate.Key.X,
                    Y = coordinate.Key.Y
                };

                if (coordinate.Value != '-' && coordinate.Value != '|' && processedMap.TryGetValue(coordinate.Key, out var direction))
                {
                    newCoordinate.Text = direction switch
                    {
                        Direction.North => "^",
                        Direction.East => ">",
                        Direction.South => "v",
                        Direction.West => "<",
                        _ => throw new ArgumentException("Unexpected direction")
                    };
                }
                else
                {
                    newCoordinate.Text = coordinate.Value.ToString();
                }

                toDraw.Add(newCoordinate);
            }
            
            return toDraw.ToArray();
        }

        private ExecutionState RunEnergizeStep(ExecutionState currentState)
        {
            var beamsAfterMoving = new List<Beam>();

            foreach (var beam in currentState.CurrentBeams)
            {
                if (beam.CurrentLocation is null)
                {
                    // Updated overall coordinates
                    currentState.EnergizedCoordinates.UnionWith(beam.VisitedCoordinates);
                    continue;
                }

                if (!currentState.AlreadyProcessed.Add(new CoordinateWithDir(beam.CurrentLocation, beam.CurrentDirection)))
                {
                    // Another beam has been here with the same direction, so stop processing it
                    currentState.EnergizedCoordinates.UnionWith(beam.VisitedCoordinates);
                    continue;
                }

                var updatedBeam = ProcessMove(beam);

                beamsAfterMoving.AddRange(updatedBeam);
            }

            return new ExecutionState
            {
                CurrentBeams = beamsAfterMoving,
                AlreadyProcessed = currentState.AlreadyProcessed,
                EnergizedCoordinates = currentState.EnergizedCoordinates,
                Started = true
            };
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

        public class Beam
        {
            public Beam(Coordinate currentLocation, Direction currentDirection)
            {
                CurrentLocation = currentLocation;
                CurrentDirection = currentDirection;
                VisitedCoordinates =
                [
                    currentLocation
                ];
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

            public HashSet<Coordinate> VisitedCoordinates { get; } = [];
        }
    }

    /// <summary>
    /// The old code mostly copied and not step based
    /// </summary>
    public class TestPuzzle : SingleExecutionPuzzle<Dictionary<Coordinate, char>>, IVisualize2d
    {
        public override PuzzleInfo Info { get; } = new(2023, 16, "The Floor Will Be Lava");

        protected override async Task<Dictionary<Coordinate, char>> LoadInputState(string puzzleInput)
        {
            coordinateMap = (await new GridFileReader().ReadFromString(puzzleInput)).ToDictionary(x => x.Coordinate, x => x.Value);
            return coordinateMap;
        }

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            totalBeamVisited.Clear();
            return await SolvePartOneInternal();
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            totalBeamVisited.Clear();
            return await SolvePartTwoInternal();
        }

        private Task<string> SolvePartOneInternal()
        {
            var maxY = coordinateMap.Keys.Max(x => x.Y);

            return Task.FromResult(GetEnergizeCount(new Beam(new Coordinate(-1, maxY), Direction.East))
                                       .ToString());
        }

        private readonly HashSet<Coordinate> totalBeamVisited = [];

        private int GetEnergizeCount(Beam startingBeam)
        {
            var beams = new List<Beam>
            {
                startingBeam
            };

            var beamVisited = new HashSet<Coordinate>();

            var handled = new HashSet<(Coordinate, Direction)>();

            while (true)
            {
                var currentBeams = beams.ToList();
                beams.Clear();

                foreach (var beam in currentBeams)
                {
                    if (beam.CurrentLocation is null)
                    {
                        beamVisited.UnionWith(beam.VisitedCoordinates);
                        continue;
                    }

                    if (handled.Contains((beam.CurrentLocation, beam.CurrentDirection)))
                    {
                        beamVisited.UnionWith(beam.VisitedCoordinates);
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
                var energizedCount = beamVisited.Count - 1;

                //coordinateMap.Keys.Draw(x =>
                //{
                //    if (visited.Contains(x))
                //    {
                //        return "#";
                //    }

                //    return coordinateMap[x]
                //        .ToString();
                //});

                lock (totalBeamVisited)
                {
                    totalBeamVisited.UnionWith(beamVisited);
                }

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

        private async Task<string> SolvePartTwoInternal()
        {
            var maxX = coordinateMap.Max(x => x.Key.X);
            var maxY = coordinateMap.Max(x => x.Key.Y);


            var topRowTask = Task.Run(() =>
            {
                var topMax = 0;
                // Top row
                for (var x = 0; x <= maxX; x++)
                {
                    var startBeam = new Beam(new Coordinate(x, maxY + 1), Direction.South);

                    var energizeCount = GetEnergizeCount(startBeam);

                    topMax = Math.Max(energizeCount, topMax);
                }

                return topMax;
            });


            var bottomRowTask = Task.Run(() =>
            {
                var bottomMax = 0;
                // Bottom row
                for (var x = 0; x <= maxX; x++)
                {
                    var startBeam = new Beam(new Coordinate(x, -1), Direction.North);

                    var energizeCount = GetEnergizeCount(startBeam);

                    bottomMax = Math.Max(energizeCount, bottomMax);
                }

                return bottomMax;
            });

            var rightSideTask = Task.Run(() =>
            {
                var rightMax = 0;
                // Right side
                for (var y = 0; y <= maxY; y++)
                {
                    var startBeam = new Beam(new Coordinate(maxX + 1, y), Direction.West);

                    var energizeCount = GetEnergizeCount(startBeam);

                    rightMax = Math.Max(energizeCount, rightMax);
                }

                return rightMax;
            });

            var leftSideTask = Task.Run(() =>
            {
                var leftMax = 0;
                // Left side
                for (var y = 0; y <= maxY; y++)
                {
                    var startBeam = new Beam(new Coordinate(-1, y), Direction.East);

                    var energizeCount = GetEnergizeCount(startBeam);

                    leftMax = Math.Max(energizeCount, leftMax);
                }

                return leftMax;
            });

            await Task.WhenAll(topRowTask, bottomRowTask, leftSideTask, rightSideTask);

            var maxValue = new[] { topRowTask.Result, bottomRowTask.Result, leftSideTask.Result, rightSideTask.Result }
                .Max();

            return maxValue.ToString();
        }

        private Dictionary<Coordinate, char> coordinateMap = [];

        private class Beam
        {
            public Beam(Coordinate loc, Direction d)
            {
                VisitedCoordinates = [loc];
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

        public DrawableCoordinate[] GetCoordinates()
        {
            // TODO want to make a helper function for stuff like this
            // Takes in predicates to pick the color or something
            return coordinateMap.Select(x => new DrawableCoordinate
                {
                    X = x.Key.X,
                    Y = x.Key.Y,
                    Text = totalBeamVisited.Contains(x.Key) ? "#" : x.Value.ToString(),
                    Color = totalBeamVisited.Contains(x.Key) ? "yellow" : "white"
                })
                .ToArray();
        }
    }
}
