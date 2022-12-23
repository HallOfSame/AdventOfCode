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
            var facing = Direction.East;

            Direction GetNewFacing(string turnDirection,
                                   Direction f)
            {
                if (turnDirection == "L")
                {
                    return f switch
                    {
                        Direction.North => Direction.West,
                        Direction.West => Direction.South,
                        Direction.South => Direction.East,
                        Direction.East => Direction.North
                    };
                }

                return f switch
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
                    facing = GetNewFacing(direction, facing);
                }
                else
                {
                    var distanceToMoveForward = int.Parse(direction);

                    (int yMod, int xMod) GetModForDirection(Direction d)
                    {
                        var yDelta = d switch
                        {
                            Direction.North => -1,
                            Direction.South => 1,
                            _ => 0
                        };

                        var xDelta = d switch
                        {
                            Direction.East => 1,
                            Direction.West => -1,
                            _ => 0
                        };

                        return (yDelta, xDelta);
                    }

                    while (distanceToMoveForward > 0)
                    {
                        var (yMod, xMod) = GetModForDirection(facing);

                        var (changeDirection, newSpace) = GetNextSpace(current,
                                                                       xMod,
                                                                       yMod,
                                                                       facing);

                        var oppositeDirection = GetNewFacing("L", changeDirection);
                        oppositeDirection = GetNewFacing("L", oppositeDirection);

                        var (yInv, xInv) = GetModForDirection(oppositeDirection);

                        // Helpful for debugging the transitions, invert the move and check we get to the same space
                        // Didn't catch everything, but did catch most things
                        var (_, inverseSpace) = GetNextSpace(newSpace,
                                                             xInv,
                                                             yInv,
                                                             oppositeDirection);

                        if (!current.Equals(inverseSpace))
                        {
                            throw new Exception("Inversion failed");
                        }

                        if (!newSpace.Passable)
                        {
                            break;
                        }

                        facing = changeDirection;

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

            var result = (rowVal + colVal + facingVal);

            if (result != 142380)
            {
                throw new Exception("Too low");
            }

            return result.ToString();
        }

        private (Direction newFacing, MapSpace newSpace) GetNextSpace(MapSpace current,
                                                                      int xMod,
                                                                      int yMod,
                                                                      Direction facing)
        {
            var next = new MapSpace
                       {
                           X = current.X + xMod,
                           Y = current.Y + yMod
                       };

            var changeDirection = facing;

            if (!map.Contains(next))
            {
                // Hard coding face transitions
                /*
                             * Input (50x50):
                                    11112222
                                    11112222
                                    11112222
                                    11112222
	                                3333
	                                3333
	                                3333
	                                3333
                                44445555
                                44445555
                                44445555
                                44445555
                                6666
                                6666
                                6666
                                6666
                             *
                             */
                // Off the bottom can only happen from 6 moving down
                if (next.Y > 200)
                {
                    // Goes to top of 2, still heading south
                    next.Y = 1;
                    next.X = next.X + 100;
                }
                else if (next.Y == 100
                         && next.X <= 50
                         && facing == Direction.North)
                {
                    // 4 going up ends up on 3 going east
                    changeDirection = Direction.East;
                    // X becomes Y
                    next.Y = 50 + next.X;
                    next.X = 51;
                }
                else if (next.X is > 50 and <= 100
                         && next.Y == 151
                         && facing == Direction.South)
                {
                    // 5 going south goes to right side of 6
                    changeDirection = Direction.West;
                    next.Y = next.X + 100;
                    next.X = 50;
                }
                else if (next.X is > 100
                         && next.Y is > 100 and <= 150
                         && facing == Direction.East)
                {
                    // 5 going East goes to right side of 2 (upside down)
                    changeDirection = Direction.West;
                    next.Y = 151 - next.Y;
                    next.X = 150;
                }
                else if (next.Y is < 1 && facing == Direction.North)
                {
                    // 1 going up
                    if (next.X <= 100)
                    {
                        // Comes to 6 going East
                        next.Y = next.X + 100;
                        next.X = 1;
                        changeDirection = Direction.East;
                    }
                    else
                    {
                        // 2 going up
                        // Comes to bottom of 6
                        next.X = next.X - 100;
                        next.Y = 200;
                    }
                }
                else if (next.X < 51
                         && next.Y is < 51
                         && facing == Direction.West)
                {
                    // 1 going west
                    // Comes to 4 going east upside down
                    changeDirection = Direction.East;
                    // Y is inverted
                    next.X = 1;
                    next.Y = 151 - next.Y;
                }
                else if (next.X > 100
                         && next.Y <= 50
                         && facing == Direction.East)
                {
                    // 2 going east
                    // Comes to 5 going west upside down
                    changeDirection = Direction.West;
                    next.X = 100;
                    next.Y = 151 - next.Y;
                }
                else if (next.X < 51
                         && next.Y is > 50 and < 101
                         && facing == Direction.West)
                {
                    // 3 going west
                    // Comes to top of 4 going south
                    changeDirection = Direction.South;
                    next.X = next.Y - 50;
                    next.Y = 101;
                }
                else if (next.X < 1 && facing == Direction.West)
                {
                    if (next.Y < 151)
                    {
                        // 4 going west
                        // Comes to left of 1 upside down
                        changeDirection = Direction.East;
                        next.X = 51;
                        next.Y = 151 - next.Y;
                    }
                    else
                    {
                        // 6 going west
                        // Comes to top of 1
                        changeDirection = Direction.South;
                        next.X = next.Y - 100;
                        next.Y = 1;
                    }
                }
                else if (next.X == 51
                         && next.Y > 150
                         && facing == Direction.East)
                {
                    // 6 going east
                    // Comes to bottom of 5
                    changeDirection = Direction.North;
                    next.X = next.Y - 100;
                    next.Y = 150;
                }
                else if (next.X == 101
                         && next.Y is > 50 and < 101
                         && facing == Direction.East)
                {
                    // 3 going east
                    // Comes to bottom of 2
                    changeDirection = Direction.North;
                    next.X = next.Y + 50;
                    next.Y = 50;
                }
                else if (next.Y > 50
                         && next.X > 100
                         && facing == Direction.South)
                {
                    // 2 going south
                    // Comes to right of 3
                    changeDirection = Direction.West;
                    next.Y = next.X - 50;
                    next.X = 100;
                }
            }

            if (!map.TryGetValue(next,
                                 out var newSpace))
            {
                throw new Exception($"Could not find {next} in map");
            }

            return (changeDirection, newSpace);
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