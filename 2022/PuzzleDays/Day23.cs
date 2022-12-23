using System.Diagnostics;

using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day23 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var map = startingPositions.Select(x => (Coordinate)x.Clone())
                                       .ToHashSet();

            //Console.WriteLine("Initial state");
            //map.Draw(_ => "#",
            //         forceOrigin: true);

            for (var round = 1; round <= 10; round++)
            {
                var unmovedElves = new HashSet<Coordinate>();

                // Key == new location
                // Value == list of elves planning to move there
                var locationProposals = new Dictionary<Coordinate, List<Coordinate>>();

                // Part one of round -> determine proposed moves
                foreach (var elf in map)
                {
                    var neighbors = elf.GetNeighbors(true);

                    var shouldMove = neighbors.Any(x => map.Contains(x));

                    bool DecisionInvalid(Decision decision)
                    {
                        return map.Contains(neighbors[(int)decision.CheckOne]) || map.Contains(neighbors[(int)decision.CheckTwo]) || map.Contains(neighbors[(int)decision.CheckThree]);
                    }

                    var madeMove = false;

                    if (shouldMove)
                    {

                        foreach (var decision in decisions)
                        {
                            if (DecisionInvalid(decision))
                            {
                                continue;
                            }

                            madeMove = true;

                            var updatedLocation = neighbors[(int)decision.MoveDirection];

                            if (!locationProposals.TryGetValue(updatedLocation,
                                                               out var existingList))
                            {
                                locationProposals[updatedLocation] = new List<Coordinate>
                                                                     {
                                                                         elf
                                                                     };
                            }
                            else
                            {
                                existingList.Add(elf);
                            }

                            break;
                        }
                    }

                    if (!madeMove)
                    {
                        unmovedElves.Add(elf);
                    }
                }

                // Part two execute moves
                var updatedMap = unmovedElves;

                foreach (var (newLocation, proposedMovers) in locationProposals)
                {
                    if (proposedMovers.Count == 1)
                    {
                        // Only one elf wants to move here, allow the move
                        updatedMap.Add(newLocation);
                    }
                    else
                    {
                        // Multiple elves plan to move here, move none of them
                        proposedMovers.ForEach(x => updatedMap.Add(x));
                    }

                    map = updatedMap;
                }

                // Rotate the decision order
                decisions = decisions.Skip(1)
                                     .Concat(decisions.Take(1))
                                     .ToArray();

                //Console.WriteLine($"End of round {round}");
                //map.Draw(_ => "#",
                //         forceOrigin: true);
            }

            var xMin = map.Min(x => x.X);
            var xMax = map.Max(x => x.X);
            var yMin = map.Min(x => x.Y);
            var yMax = map.Max(x => x.Y);

            var xLength = xMax - xMin + 1;
            var yLength = yMax - yMin + 1;

            var area = xLength * yLength;

            var emptySpaces = area - map.Count;

            return emptySpaces.ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
                        var map = startingPositions.Select(x => (Coordinate)x.Clone())
                                       .ToHashSet();

            //Console.WriteLine("Initial state");
            //map.Draw(_ => "#",
            //         forceOrigin: true);

            var round = 0;

            while(true)
            {
                round++;

                var unmovedElves = new HashSet<Coordinate>();

                // Key == new location
                // Value == list of elves planning to move there
                var locationProposals = new Dictionary<Coordinate, List<Coordinate>>();

                // Part one of round -> determine proposed moves
                foreach (var elf in map)
                {
                    var neighbors = elf.GetNeighbors(true);

                    var shouldMove = neighbors.Any(x => map.Contains(x));

                    bool DecisionInvalid(Decision decision)
                    {
                        return map.Contains(neighbors[(int)decision.CheckOne]) || map.Contains(neighbors[(int)decision.CheckTwo]) || map.Contains(neighbors[(int)decision.CheckThree]);
                    }

                    var madeMove = false;

                    if (shouldMove)
                    {

                        foreach (var decision in decisions)
                        {
                            if (DecisionInvalid(decision))
                            {
                                continue;
                            }

                            madeMove = true;

                            var updatedLocation = neighbors[(int)decision.MoveDirection];

                            if (!locationProposals.TryGetValue(updatedLocation,
                                                               out var existingList))
                            {
                                locationProposals[updatedLocation] = new List<Coordinate>
                                                                     {
                                                                         elf
                                                                     };
                            }
                            else
                            {
                                existingList.Add(elf);
                            }

                            break;
                        }
                    }

                    if (!madeMove)
                    {
                        unmovedElves.Add(elf);
                    }
                }

                // Part two execute moves
                var updatedMap = unmovedElves;

                var hadMove = false;

                foreach (var (newLocation, proposedMovers) in locationProposals)
                {
                    if (proposedMovers.Count == 1)
                    {
                        // Only one elf wants to move here, allow the move
                        updatedMap.Add(newLocation);

                        hadMove = true;
                    }
                    else
                    {
                        // Multiple elves plan to move here, move none of them
                        proposedMovers.ForEach(x => updatedMap.Add(x));
                    }

                    map = updatedMap;
                }

                if (!hadMove)
                {
                    break;
                }

                // Rotate the decision order
                decisions = decisions.Skip(1)
                                     .Concat(decisions.Take(1))
                                     .ToArray();

                //Console.WriteLine($"End of round {round}");
                //map.Draw(_ => "#",
                //         forceOrigin: true);
            }

            if (round == 1139)
            {
                throw new Exception("Too high");
            }

            return round.ToString();
        }

        public override async Task ReadInput()
        {
            var lines = await new StringFileReader().ReadInputFromFile();

            var y = lines.Count - 1;

            foreach (var line in lines)
            {
                for (var x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                    {
                        startingPositions.Add(new Coordinate(x,
                                                             y));
                    }
                }

                y--;
            }
        }

        private HashSet<Coordinate> startingPositions = new();

        private Decision[] decisions =
        {
            new Decision
            {
                MoveDirection = Direction.North,
                CheckOne = Direction.North,
                CheckTwo = Direction.NorthEast,
                CheckThree = Direction.NorthWest
            },
            new Decision
            {
                MoveDirection = Direction.South,
                CheckOne = Direction.South,
                CheckTwo = Direction.SouthEast,
                CheckThree = Direction.SouthWest
            },
            new Decision
            {
                MoveDirection = Direction.West,
                CheckOne = Direction.West,
                CheckTwo = Direction.SouthWest,
                CheckThree = Direction.NorthWest
            },
            new Decision
            {
                MoveDirection = Direction.East,
                CheckOne = Direction.East,
                CheckTwo = Direction.NorthEast,
                CheckThree = Direction.SouthEast
            },
        };
    }
}

[DebuggerDisplay("Move {MoveDirection}")]
class Decision
{
    public Direction MoveDirection { get; set; }

    public Direction CheckOne { get; set; }

    public Direction CheckTwo { get; set; }

    public Direction CheckThree { get; set; }
}