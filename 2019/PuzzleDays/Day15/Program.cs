using Helpers.Coordinates;

using IntCodeInterpreter;
using IntCodeInterpreter.Input;

using Spectre.Console;

var controlProgram = await new FileInputParser().ReadOperationsFromFile("PuzzleInput.txt");

var manager = new RobotManager(controlProgram);

manager.Run();

class RobotManager
{
    private readonly List<long> controlProgram;

    private readonly Interpreter interpreter = new();

    public RobotManager(List<long> controlProgram)
    {
        this.controlProgram = controlProgram;
    }

    public void Run()
    {
        var breakExecution = false;

        currentCoordinate = new Coordinate2D(0,
                                             0);

        // Assuming we start in an open spot
        tileMap[currentCoordinate] = TileType.Empty;

        var stepCount = 0;

        try
        {
            interpreter.ProcessOperations(controlProgram,
                                          () =>
                                          {
                                              if (stepCount % 100 == 0)
                                              {
                                                  DrawCurrentMap($"Step {stepCount}");
                                              }

                                              stepCount++;

                                              if (breakExecution)
                                              {
                                                  throw new ApplicationException("Killing process, signaled to stop,");
                                              }

                                              var nextInput = GetNextInputCommand();

                                              lastInput = (Direction)nextInput;

                                              return nextInput;
                                          },
                                          x =>
                                          {
                                              if (ProcessOutput(x))
                                              {
                                                  breakExecution = true;
                                              }
                                          });
        }
        catch (ApplicationException)
        {
            // Expected eventually
        }

        DrawCurrentMap("Program End");

        var distance = CalculateDistanceFromOriginToO2();

        AnsiConsole.MarkupLine($"[yellow]Distance from origin to O2 System: {distance}.[/]");
    }

    private int CalculateDistanceFromOriginToO2()
    {
        // A* search

        // Using manhattan distance to estimate the remaining distance to the end
        int ManhattanDistanceToO2System(Coordinate2D current)
        {
            return Math.Abs(current.X - o2Location.X) + Math.Abs(current.Y - o2Location.Y);
        }

        var start = new Coordinate2D(0, 0);

        var workingSet = new HashSet<Coordinate2D>
                         {
                             start
                         };

        // cameFromMap[x] == the cheapest coordinate to reach it from the start location
        // Can be used to rebuild the pathway
        var cameFromMap = new Dictionary<Coordinate2D, Coordinate2D>();

        // gScoreMap[x] == the cost of the cheapest path from start to x
        var gScoreMap = tileMap.ToDictionary(x => x.Key,
                                             x => int.MaxValue);

        // fScoreMap[x] == our guess to how far x is from the start, using gScore + a guesstimate
        var fScoreMap = gScoreMap.ToDictionary(x => x.Key,
                                               x => x.Value);

        gScoreMap[start] = 0;
        fScoreMap[start] = ManhattanDistanceToO2System(start);

        while (workingSet.Any())
        {
            var current = workingSet.OrderBy(x => fScoreMap[x])
                                    .First();

            if (Equals(current,
                       o2Location))
            {
                // We reached the O2 system, return the number of moves needed
                return gScoreMap[current];
            }

            workingSet.Remove(current);

            foreach (var neighbor in GetNeighborCoordinates(current)
                         .Where(x => tileMap.ContainsKey(x) && !blockedCoordinates.Contains(x)))
            {
                var tentativeGScore = gScoreMap[current] + 1; // Distance from a neighbor is always 1

                // This path is better, so record it
                if (tentativeGScore < gScoreMap[neighbor])
                {
                    // Record current best way to reach neighbor
                    cameFromMap[neighbor] = current;
                    // Update the scores
                    gScoreMap[neighbor] = tentativeGScore;
                    fScoreMap[neighbor] = tentativeGScore + ManhattanDistanceToO2System(neighbor);
                    // Ensure neighbor is in the working set again
                    workingSet.Add(neighbor);
                }
            }
        }

        throw new InvalidOperationException("Path from origin to O2 system could not be located.");
    }

    private Direction lastInput;

    private Coordinate2D currentCoordinate;

    private readonly Dictionary<Coordinate2D, TileType> tileMap = new();

    private Coordinate2D GetUpdatedCoordinate(Direction directionMoved)
    {
        var xDiff = 0;
        var yDiff = 0;

        switch (directionMoved)
        {
            case Direction.North:
                yDiff = 1;
                break;
            case Direction.South:
                yDiff = -1;
                break;
            case Direction.East:
                xDiff = 1;
                break;
            case Direction.West:
                xDiff = -1;
                break;
        }

        return new Coordinate2D(currentCoordinate.X + xDiff, currentCoordinate.Y + yDiff);
    }

    private bool ProcessOutput(long output)
    {
        var tileType = output switch
        {
            0 => TileType.Wall,
            1 => TileType.Empty,
            2 => TileType.O2System,
            _ => throw new ArgumentOutOfRangeException()
        };

        var targetCoordinate = GetUpdatedCoordinate(lastInput);

        tileMap[targetCoordinate] = tileType;

        if (tileType != TileType.Wall)
        {
            currentCoordinate = targetCoordinate;

            if (!backtracking)
            {
                moveStack.Push(lastInput);
            }
        }
        else
        {
            blockedCoordinates.Add(targetCoordinate);
        }

        // Found the O2 System
        if (tileType == TileType.O2System)
        {
            o2Location = targetCoordinate;
        }

        return false;
    }

    private Coordinate2D o2Location;

    private void DrawCurrentMap(string title)
    {
        var minX = tileMap.Min(x => x.Key.X);
        var maxX = tileMap.Max(x => x.Key.X);
        var minY = tileMap.Min(x => x.Key.Y);
        var maxY = tileMap.Max(x => x.Key.Y);

        var xAdjust = minX;
        var yAdjust = maxY;

        var canvas = new Canvas(maxX - minX + 1,
                                maxY - minY + 1);

        foreach (var (coordinate2D, tileType) in tileMap)
        {
            var color = tileType switch
            {
                TileType.Wall => Color.Red,
                TileType.Empty => Color.Blue,
                TileType.O2System => Color.Green,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (coordinate2D.Equals(currentCoordinate))
            {
                // Draw white normally
                // If we're on the target, draw gold
                color = color == Color.Green
                            ? Color.Gold1
                            : Color.White;
            }

            canvas.SetPixel(coordinate2D.X - xAdjust,
                            (coordinate2D.Y - yAdjust) * -1,
                            color);
        }

        // Mark the start location
        canvas.SetPixel(0 - xAdjust,
                        (0 - yAdjust) * -1,
                        Color.Fuchsia);

        var panel = new Panel(canvas)
                    {
                        Header = new PanelHeader($"Current Map: {title}")
                    };

        AnsiConsole.Write(panel);
    }

    private List<Coordinate2D> GetNeighborCoordinates(Coordinate2D coordinate)
    {
        return new List<Coordinate2D>
               {
                   new(coordinate.X - 1,
                       coordinate.Y),
                   new(coordinate.X + 1,
                       coordinate.Y),
                   new(coordinate.X,
                       coordinate.Y - 1),
                   new(coordinate.X,
                       coordinate.Y + 1),
               };
    }

    private readonly HashSet<Coordinate2D> visitedCoordinates = new ();

    private readonly HashSet<Coordinate2D> blockedCoordinates = new();

    private readonly Stack<Direction> moveStack = new();

    private bool backtracking = false;

    private long GetNextInputCommand()
    {
        var neighborsOfCurrent = GetNeighborCoordinates(currentCoordinate);

        var nextCoordinateToTry = neighborsOfCurrent.FirstOrDefault(x => !visitedCoordinates.Contains(x) && !blockedCoordinates.Contains(x));

        if (nextCoordinateToTry == null)
        {
            if (moveStack.Count == 0)
            {
                throw new ApplicationException("Searched entire map!");
            }

            backtracking = true;

            // Backtrack by pulling off our list of moves
            return (long)(moveStack.Pop() switch
                             {
                                 Direction.North => Direction.South,
                                 Direction.South => Direction.North,
                                 Direction.West => Direction.East,
                                 Direction.East => Direction.West,
                                 _ => throw new ArgumentOutOfRangeException()
                             });
        }

        backtracking = false;

        visitedCoordinates.Add(nextCoordinateToTry);

        if (nextCoordinateToTry.X == currentCoordinate.X)
        {
            if (nextCoordinateToTry.Y > currentCoordinate.Y)
            {
                return (long)Direction.North;
            }

            return (long)Direction.South;
        }

        if (nextCoordinateToTry.X > currentCoordinate.X)
        {
            return (long)Direction.East;
        }

        return (long)Direction.West;
    }
}

enum Direction
{
    North = 1,
    South = 2,
    West = 3,
    East = 4
}

enum TileType
{
    Empty = 0,
    Wall = 1,
    O2System = 2
}