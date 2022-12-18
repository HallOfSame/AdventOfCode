using System.Diagnostics;
using System.Text;

using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day17 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var rockIndex = 0;

            var moveDownThisTurn = false;

            var topRockY = -1;

            var filledCoordinates = new HashSet<Coordinate>();

            Rock? currentRock = null;

            var jetIndex = 0;

            var stoppedRocks = 0;

            bool IsValidMove(HashSet<Coordinate> coordinates)
            {
                var minX = coordinates.Min(x => x.X);
                var maxX = coordinates.Max(x => x.X);

                var minY = coordinates.Min(y => y.Y);

                if (minX < 0
                    || maxX > 6)
                {
                    return false;
                }

                if (minY < 0)
                {
                    return false;
                }

                return coordinates.All(x => !filledCoordinates.Contains(x));
            }

            while (true)
            {
                if (stoppedRocks == 2022)
                {
                    break;
                }

                if (currentRock is null)
                {
                    var newRockSpawnCoord = new Coordinate(2,
                                                           topRockY + 4);

                    currentRock = (Shape)rockIndex switch
                    {
                        Shape.Line => new LineRock(),
                        Shape.Cross => new CrossRock(),
                        Shape.L => new LRock(),
                        Shape.VertLine => new VertLineRock(),
                        Shape.Square => new SquareRock(),
                        _ => throw new ArgumentException("Unexpected rock index")
                    };

                    rockIndex++;

                    if (rockIndex == 5)
                    {
                        rockIndex = 0;
                    }

                    currentRock.Create(newRockSpawnCoord);

                    moveDownThisTurn = false;

                    //Draw(filledCoordinates,
                    //     currentRock.Location);
                }

                if (!moveDownThisTurn)
                {
                    var direction = jetMovement[jetIndex] == '<'
                                        ? RockDirection.Left
                                        : RockDirection.Right;

                    jetIndex++;

                    if (jetIndex == jetMovement.Length)
                    {
                        jetIndex = 0;
                    }

                    var updatedLocation = currentRock.GetMoveLocation(direction);

                    if (IsValidMove(updatedLocation))
                    {
                        currentRock.Location = updatedLocation;
                    }
                }
                else
                {
                    var updatedLocation = currentRock.GetMoveLocation(RockDirection.Down);

                    if (IsValidMove(updatedLocation))
                    {
                        currentRock.Location = updatedLocation;
                    }
                    else
                    {
                        filledCoordinates.UnionWith(currentRock.Location);
                        topRockY = Math.Max(topRockY,
                                            currentRock.Location.Max(x => x.Y));
                        currentRock = null;
                        stoppedRocks++;
                    }
                }

                moveDownThisTurn = !moveDownThisTurn;
            }

            return (filledCoordinates.Max(x => x.Y) + 1).ToString();
        }

        private void Draw(HashSet<Coordinate> filledCoordinates,
                          HashSet<Coordinate> currentRock)
        {
            var maxY = Math.Max(filledCoordinates.Any()
                                    ? filledCoordinates.Max(x => x.Y)
                                    : 0,
                                currentRock.Max(x => x.Y));

            var sb = new StringBuilder();

            for (var y = maxY; y > -1; y--)
            {
                sb.Append('|');

                for (var x = 0; x < 7; x++)
                {
                    if (filledCoordinates.Contains(new Coordinate(x,
                                                                  y)))
                    {
                        sb.Append('#');
                    }
                    else if (currentRock.Contains(new Coordinate(x,
                                                                 y)))
                    {
                        sb.Append('@');
                    }
                    else
                    {
                        sb.Append('.');
                    }
                }

                sb.Append('|');
                sb.Append(Environment.NewLine);
            }

            sb.AppendLine(new string(Enumerable.Repeat('-',
                                                       9)
                                               .ToArray()));

            Console.WriteLine(sb.ToString());
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var floor = -1;

            var rockIndex = 0;

            var moveDownThisTurn = false;

            var topRockY = -1;

            var filledCoordinates = new HashSet<Coordinate>();

            Rock? currentRock = null;

            var jetIndex = 0;

            var stoppedRocks = 0;

            bool IsValidMove(HashSet<Coordinate> coordinates)
            {
                var minX = coordinates.Min(x => x.X);
                var maxX = coordinates.Max(x => x.X);

                var minY = coordinates.Min(y => y.Y);

                if (minX < 0
                    || maxX > 6)
                {
                    return false;
                }

                if (minY < 0)
                {
                    return false;
                }

                return coordinates.All(x => !filledCoordinates.Contains(x));
            }

            // Detect cycle (at rock spawn) based on:
            // height in each column relative to highest rock
            // Current rock type being spawned
            // Index in jet
            var setupCodes = new Dictionary<string, (int spawnCount, int currentHeight)>();

            string cycleCode;

            while (true)
            {
                if (currentRock is null)
                {
                    var newRockSpawnCoord = new Coordinate(2,
                                                           topRockY + 4);

                    currentRock = (Shape)rockIndex switch
                    {
                        Shape.Line => new LineRock(),
                        Shape.Cross => new CrossRock(),
                        Shape.L => new LRock(),
                        Shape.VertLine => new VertLineRock(),
                        Shape.Square => new SquareRock(),
                        _ => throw new ArgumentException("Unexpected rock index")
                    };

                    rockIndex++;

                    if (rockIndex == 5)
                    {
                        rockIndex = 0;
                    }

                    currentRock.Create(newRockSpawnCoord);

                    moveDownThisTurn = false;

                    var code = $"{rockIndex},{jetIndex},";

                    for (var i = 0; i < 7; i++)
                    {
                        var columnCoordinates = filledCoordinates.Where(x => x.X == i);

                        var columnDelta = (columnCoordinates.Any()
                                               ? columnCoordinates.Max(x => x.Y)
                                               : 0)
                                          - topRockY;

                        code += columnDelta + ",";
                    }

                    if (setupCodes.ContainsKey(code))
                    {
                        cycleCode = code;
                        break;
                    }

                    setupCodes.Add(code,
                                   (stoppedRocks, topRockY));

                    //Draw(filledCoordinates,
                    //     currentRock.Location);
                }

                if (!moveDownThisTurn)
                {
                    var direction = jetMovement[jetIndex] == '<'
                                        ? RockDirection.Left
                                        : RockDirection.Right;

                    jetIndex++;

                    if (jetIndex == jetMovement.Length)
                    {
                        jetIndex = 0;
                    }

                    var updatedLocation = currentRock.GetMoveLocation(direction);

                    if (IsValidMove(updatedLocation))
                    {
                        currentRock.Location = updatedLocation;
                    }
                }
                else
                {
                    var updatedLocation = currentRock.GetMoveLocation(RockDirection.Down);

                    if (IsValidMove(updatedLocation))
                    {
                        currentRock.Location = updatedLocation;
                    }
                    else
                    {
                        filledCoordinates.UnionWith(currentRock.Location);
                        topRockY = Math.Max(topRockY,
                                            currentRock.Location.Max(x => x.Y));
                        currentRock = null;
                        stoppedRocks++;
                    }
                }

                moveDownThisTurn = !moveDownThisTurn;
            }

            // Figure out what the cycle is like
            var cycleInfo = setupCodes[cycleCode];

            // Length == how many rocks spawn in a cycle
            var cycleLength = stoppedRocks - cycleInfo.spawnCount;

            // Height == how much height is added per cycle
            var cycleHeight = topRockY - cycleInfo.currentHeight;

            // Calculate how many cycles we need to run to get close to 1 tril
            var cycles = Math.Floor((1_000_000_000_000m - cycleInfo.spawnCount) / cycleLength);

            // Calculate how much height all those cycles would add
            var currentHeight = topRockY + 1;

            var heightAfterCycling = (cycles * cycleHeight) + cycleInfo.currentHeight + 1;
            var topAtCycle = currentHeight;

            // Then figure out how many more rocks need to drop to get to an even 1 tril
            var remainingCycles = (1_000_000_000_000m - cycleInfo.spawnCount) % cycleLength;

            stoppedRocks = 0;
            
            // Repetitive but I'm too lazy to extract a function right now
            while (true)
            {
                if (stoppedRocks == remainingCycles)
                {
                    break;
                }

                if (currentRock is null)
                {
                    var newRockSpawnCoord = new Coordinate(2,
                                                           topRockY + 4);

                    currentRock = (Shape)rockIndex switch
                    {
                        Shape.Line => new LineRock(),
                        Shape.Cross => new CrossRock(),
                        Shape.L => new LRock(),
                        Shape.VertLine => new VertLineRock(),
                        Shape.Square => new SquareRock(),
                        _ => throw new ArgumentException("Unexpected rock index")
                    };

                    rockIndex++;

                    if (rockIndex == 5)
                    {
                        rockIndex = 0;
                    }

                    currentRock.Create(newRockSpawnCoord);

                    moveDownThisTurn = false;

                    //Draw(filledCoordinates,
                    //     currentRock.Location);
                }

                if (!moveDownThisTurn)
                {
                    var direction = jetMovement[jetIndex] == '<'
                                        ? RockDirection.Left
                                        : RockDirection.Right;

                    jetIndex++;

                    if (jetIndex == jetMovement.Length)
                    {
                        jetIndex = 0;
                    }

                    var updatedLocation = currentRock.GetMoveLocation(direction);

                    if (IsValidMove(updatedLocation))
                    {
                        currentRock.Location = updatedLocation;
                    }
                }
                else
                {
                    var updatedLocation = currentRock.GetMoveLocation(RockDirection.Down);

                    if (IsValidMove(updatedLocation))
                    {
                        currentRock.Location = updatedLocation;
                    }
                    else
                    {
                        filledCoordinates.UnionWith(currentRock.Location);
                        topRockY = Math.Max(topRockY,
                                            currentRock.Location.Max(x => x.Y));
                        currentRock = null;
                        stoppedRocks++;
                    }
                }

                moveDownThisTurn = !moveDownThisTurn;
            }

            // Add our cyclying height to the last bit of simulation we did
            var result = (topRockY + 1) - topAtCycle + heightAfterCycling;

            return result.ToString();
        }

        public override async Task ReadInput()
        {
            var lines = await new StringFileReader().ReadInputFromFile();

            jetMovement = lines.First();
        }

        private string jetMovement;
    }
}

abstract class Rock
{
    public abstract void Create(Coordinate leftEdge);

    public abstract HashSet<Coordinate> Location { get; set; }

    public abstract Shape Shape { get; set; }

    public HashSet<Coordinate> GetMoveLocation(RockDirection direction)
    {
        var xDiff = direction switch
        {
            RockDirection.Left => -1,
            RockDirection.Right => 1,
            RockDirection.Down => 0,
            _ => throw new ArgumentException(nameof(direction))
        };

        var yDiff = direction == RockDirection.Down
                        ? -1
                        : 0;

        return Location.Select(x =>
                               {
                                   var updated = (Coordinate)x.Clone();

                                   updated.X += xDiff;
                                   updated.Y += yDiff;

                                   return updated;
                               })
                       .ToHashSet();
    }
}

class Code
{
    public int RockIndex { get; set; }

    public int JetIndex { get; set; }

    public string ColumnDeltas { get; set; }
}

class LineRock : Rock
{
    public override void Create(Coordinate leftEdge)
    {
        Location = new HashSet<Coordinate>();

        var left = (Coordinate)leftEdge.Clone();

        Location.Add(left);

        for (var i = 1; i <= 3; i++)
        {
            var newCoord = (Coordinate)left.Clone();

            newCoord.X += i;

            Location.Add(newCoord);
        }
    }

    public override HashSet<Coordinate> Location { get; set; }

    public override Shape Shape { get; set; } = Shape.Line;
}

class CrossRock : Rock
{
    public override void Create(Coordinate leftEdge)
    {
        Location = new HashSet<Coordinate>();

        var left = (Coordinate)leftEdge.Clone();
        // Our target coord is one above the "bottom edge"
        left.Y += 1;

        Location.Add(left);

        var center = (Coordinate)left.Clone();
        center.X += 1;

        Location.Add(center);

        var centerUp = (Coordinate)center.Clone();
        centerUp.Y += 1;

        Location.Add(centerUp);

        var centerDown = (Coordinate)center.Clone();
        centerDown.Y -= 1;

        Location.Add(centerDown);

        var right = (Coordinate)center.Clone();
        right.X += 1;

        Location.Add(right);
    }

    public override HashSet<Coordinate> Location { get; set; }

    public override Shape Shape { get; set; } = Shape.Cross;
}

class LRock : Rock
{
    public override void Create(Coordinate leftEdge)
    {
        Location = new HashSet<Coordinate>();

        var left = (Coordinate)leftEdge.Clone();

        Location.Add(left);

        for (var i = 1; i <= 2; i++)
        {
            var newCoord = (Coordinate)left.Clone();
            newCoord.X += i;
            Location.Add(newCoord);

            var vertCoord = (Coordinate)left.Clone();
            vertCoord.X += 2;
            vertCoord.Y += i;
            Location.Add(vertCoord);
        }
    }

    public override HashSet<Coordinate> Location { get; set; }

    public override Shape Shape { get; set; } = Shape.L;
}

class VertLineRock : Rock
{
    public override void Create(Coordinate leftEdge)
    {
        Location = new HashSet<Coordinate>();

        var left = (Coordinate)leftEdge.Clone();

        Location.Add(left);

        for (var i = 1; i <= 3; i++)
        {
            var newCoord = (Coordinate)left.Clone();
            newCoord.Y += i;
            Location.Add(newCoord);
        }
    }

    public override HashSet<Coordinate> Location { get; set; }

    public override Shape Shape { get; set; } = Shape.VertLine;
}

class SquareRock : Rock
{
    public override void Create(Coordinate leftEdge)
    {
        Location = new HashSet<Coordinate>();

        var left = (Coordinate)leftEdge.Clone();

        Location.Add(left);

        var rightBottom = (Coordinate)left.Clone();
        rightBottom.X += 1;
        Location.Add(rightBottom);

        var rightTop = (Coordinate)rightBottom.Clone();
        rightTop.Y += 1;
        Location.Add(rightTop);

        var leftTop = (Coordinate)rightTop.Clone();
        leftTop.X -= 1;
        Location.Add(leftTop);
    }

    public override HashSet<Coordinate> Location { get; set; }

    public override Shape Shape { get; set; } = Shape.Square;
}

enum Shape
{
    Line = 0,
    Cross = 1,
    L = 2,
    VertLine = 3,
    Square = 4
}

enum RockDirection
{
    Left,
    Right,
    Down
}