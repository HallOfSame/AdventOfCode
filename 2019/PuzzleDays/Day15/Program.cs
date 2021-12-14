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

                                              lastMove = (Direction)nextInput;

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
    }

    private Direction lastMove;

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

        var targetCoordinate = GetUpdatedCoordinate(lastMove);

        tileMap[targetCoordinate] = tileType;

        if (tileType != TileType.Wall)
        {
            currentCoordinate = targetCoordinate;
        }

        // Found the O2 System
        if (tileType == TileType.O2System)
        {
            return true;
        }

        return false;
    }

    private void DrawCurrentMap(string title)
    {
        var minX = tileMap.Min(x => x.Key.X);
        var maxX = tileMap.Max(x => x.Key.X);
        var minY = tileMap.Min(x => x.Key.Y);
        var maxY = tileMap.Max(x => x.Key.Y);

        var xAdjust = 0;
        var yAdjust = 0;

        if (minY < 0)
        {
            yAdjust = Math.Abs(minY);
        }

        if (minX < 0)
        {
            xAdjust = Math.Abs(minX);
        }

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

            canvas.SetPixel(coordinate2D.X + xAdjust,
                            coordinate2D.Y + yAdjust,
                            color);
        }

        // Mark the start location
        canvas.SetPixel(xAdjust,
                        yAdjust,
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

    private readonly HashSet<Coordinate2D> deadEndCoordinates = new ();

    private long GetNextInputCommand()
    {
        var neighborsOfCurrent = GetNeighborCoordinates(currentCoordinate);

        if (neighborsOfCurrent.Select(x => new
                                           {
                                               DeadEnd = deadEndCoordinates.Contains(x),
                                               Type = tileMap.TryGetValue(x,
                                                                          out var tileType)
                                                          ? tileType
                                                          : TileType.Empty
                                           })
                              .Count(x => x.Type == TileType.Wall || x.DeadEnd)
            == 3)
        {
            deadEndCoordinates.Add(currentCoordinate);
        }

        var nextCoordinateToTry = neighborsOfCurrent.OrderBy(x =>
                                                             {
                                                                 if (deadEndCoordinates.Contains(x))
                                                                 {
                                                                     // Don't go to dead ends
                                                                     return 99;
                                                                 }

                                                                 if (!tileMap.TryGetValue(x,
                                                                                          out var tileType))
                                                                 {
                                                                     // Prefer new locations first
                                                                     return 0;
                                                                 }

                                                                 // Otherwise, trying to get back to an empty space we can move around from
                                                                 return tileType == TileType.Empty
                                                                            ? 1
                                                                            : 99;
                                                             })
                                                    .First();

        if (nextCoordinateToTry.X == currentCoordinate.X)
        {
            if (nextCoordinateToTry.Y > currentCoordinate.Y)
            {
                return (long)Direction.North;
            }

            return (long)Direction.South;
        }
        else
        {
            if (nextCoordinateToTry.X > currentCoordinate.X)
            {
                return (long)Direction.East;
            }

            return (long)Direction.West;
        }
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