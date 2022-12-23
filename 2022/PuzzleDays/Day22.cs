using System.Text.RegularExpressions;

using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day22 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var facing = Direction.East;

            Direction GetNewFacing(string turnDirection)
            {
                if (turnDirection == "L")
                {
                    return facing switch
                    {
                        Direction.North => Direction.West,
                        Direction.West => Direction.South,
                        Direction.South => Direction.East,
                        Direction.East => Direction.North
                    };
                }

                return facing switch
                {
                    Direction.North => Direction.East,
                    Direction.East => Direction.South,
                    Direction.South => Direction.West,
                    Direction.West => Direction.North
                };
            }

            var current = start!;

            foreach (var direction in directions)
            {
                if (direction is "L" or "R")
                {
                    facing = GetNewFacing(direction);
                }
                else
                {
                    var distanceToMoveForward = int.Parse(direction);

                    var yMod = facing switch
                    {
                        Direction.North => -1,
                        Direction.South => 1,
                        _ => 0
                    };

                    var xMod = facing switch
                    {
                        Direction.East => 1,
                        Direction.West => -1,
                        _ => 0
                    };

                    while (distanceToMoveForward > 0)
                    {
                        var next = new MapSpace
                                   {
                                       X = current.X + xMod,
                                       Y = current.Y + yMod
                                   };

                        if (!map.Contains(next))
                        {
                            if (yMod == 0)
                            {
                                var sameRow = map.Where(x => x.Y == current.Y);

                                next = facing == Direction.West
                                           ? sameRow.MaxBy(x => x.X)
                                           : sameRow.MinBy(x => x.X);
                            }
                            else
                            {
                                var sameColumn = map.Where(x => x.X == current.X);

                                next = facing == Direction.North
                                           ? sameColumn.MaxBy(x => x.Y)
                                           : sameColumn.MinBy(x => x.Y);
                            }
                        }

                        if (!map.TryGetValue(next,
                                             out var newSpace))
                        {
                            throw new Exception($"Could not find {next} in map");
                        }

                        if (!newSpace.Passable)
                        {
                            break;
                        }

                        current = newSpace;
                        distanceToMoveForward--;
                    }
                }
            }

            var rowVal = 1000 * current.Y;

            var colVal = 4 * current.X;

            var facingVal = facing switch
            {
                Direction.East => 0,
                Direction.South => 1,
                Direction.West => 2,
                Direction.North => 3
            };

            return (rowVal + colVal + facingVal).ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        public override async Task ReadInput()
        {
            var lines = await new StringFileReader().ReadInputFromFile();

            var y = 1;

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }

                for (var i = 0; i < line.Length; i++)
                {
                    if (line[i] == ' ')
                    {
                        continue;
                    }

                    var x = i + 1;

                    var newSpace = new MapSpace
                                   {
                                       X = x,
                                       Y = y,
                                       Passable = line[i] != '#'
                                   };

                    map.Add(newSpace);

                    if (start == null
                        && newSpace.Passable)
                    {
                        start = newSpace;
                    }
                }

                y++;
            }

            directions = Regex.Split(lines.Last(),
                                     "(L|R)");
        }

        private string[] directions;

        private readonly HashSet<MapSpace> map = new();

        private MapSpace? start;
    }
}

class MapSpace : Coordinate
{
    public bool Passable { get; set; }
}