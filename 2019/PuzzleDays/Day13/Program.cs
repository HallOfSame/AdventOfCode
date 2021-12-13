using System.Text;

using IntCodeInterpreter;
using IntCodeInterpreter.Input;

var gameCode = await new FileInputParser().ReadOperationsFromFile("PuzzleInput.txt");

var game = new ArcadeGame(gameCode);

game.Run();

class ArcadeGame
{
    private readonly List<long> gameCode;

    private Interpreter interpreter;

    private long score;

    public ArcadeGame(List<long> gameCode)
    {
        gameCode[0] = 2; // 2 Quarters to run the game, set to 0 for part 1
        this.gameCode = gameCode;
        interpreter = new Interpreter();
        gameMap = new Dictionary<Coordinate, GameTile>();
    }

    public void Run()
    {
        var outputBuffer = new List<long>(3);

        interpreter.ProcessOperations(gameCode,
                                      () =>
                                      {
                                          DrawScreen();

                                          //Thread.Sleep(200);

                                          return ballTile.Position.X.CompareTo(paddleTile.Position.X);

                                          /*
                                           Comment out to actually play, but that's way too hard at the terrible FPS
                                          var input = Console.ReadKey();

                                          if (input.Key == ConsoleKey.LeftArrow)
                                          {
                                              return -1;
                                          }

                                          if (input.Key == ConsoleKey.DownArrow)
                                          {
                                              return 0;
                                          }

                                          if (input.Key == ConsoleKey.RightArrow)
                                          {
                                              return 1;
                                          }

                                          // Default to not moved
                                          return 0;
                                          */
                                      },
                                      (outputValue) =>
                                      {
                                          outputBuffer.Add(outputValue);

                                          if (outputBuffer.Count == 3)
                                          {
                                              ProcessOutput(outputBuffer);
                                              outputBuffer.Clear();
                                          }
                                      });

        Console.WriteLine($"Number of block tiles: {gameMap.Count(x => x.Value.Type == TileType.Block)}.");

        Console.WriteLine($"Score: {score}.");
    }

    private GameTile ballTile;

    private GameTile paddleTile;

    private void DrawScreen()
    {
        var minX = gameMap.Min(x => x.Key.X);
        var maxX = gameMap.Max(x => x.Key.X);
        var minY = gameMap.Min(y => y.Key.Y);
        var maxY = gameMap.Max(y => y.Key.Y);

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine();

        void DrawBorder()
        {
            stringBuilder.AppendLine(new string(Enumerable.Repeat('-', maxX - minX).ToArray()));
        }

        DrawBorder();

        for (var y = maxY; y >= minY; y--)
        {
            for (var x = minX; x <= maxX; x++)
            {
                var coordinate = new Coordinate
                                 {
                                     X = x,
                                     Y = y
                                 };

                if (gameMap.TryGetValue(coordinate,
                                        out var tile))
                {

                    var character = tile.Type switch
                    {
                        TileType.Block => '#',
                        TileType.Wall => '&',
                        TileType.Paddle => '=',
                        TileType.Ball => 'O',
                        TileType.Empty => ' ',
                        _ => throw new ArgumentOutOfRangeException(nameof(tile.Type))
                    };

                    stringBuilder.Append(character);
                }
                else
                {
                    stringBuilder.Append(' ');
                }
            }

            stringBuilder.Append(Environment.NewLine);
        }

        DrawBorder();
        stringBuilder.AppendLine();

        Console.WriteLine(stringBuilder.ToString());
    }

    private void ProcessOutput(List<long> outputData)
    {
        var coordinate = new Coordinate
                         {
                             X = (int)outputData[0],
                             Y = (int)outputData[1]
                         };

        if (coordinate.X == -1
            && coordinate.Y == 0)
        {
            score = outputData[2];
            return;
        }

        var tile = new GameTile
                   {
                       Position = coordinate,
                       Type = (TileType)(int)outputData[2]
                   };

        if (tile.Type == TileType.Ball)
        {
            ballTile = tile;
        }

        if (tile.Type == TileType.Paddle)
        {
            paddleTile = tile;
        }

        gameMap[coordinate] = tile;
    }

    private Dictionary<Coordinate, GameTile> gameMap;
}

class GameTile
{
    public Coordinate Position { get; set; }

    public TileType Type { get; set; }

    protected bool Equals(GameTile other)
    {
        return Position.Equals(other.Position);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null,
                            obj))
        {
            return false;
        }

        if (ReferenceEquals(this,
                            obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((GameTile)obj);
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }
}

enum TileType
{
    Empty = 0,
    Wall = 1,
    Block = 2,
    Paddle = 3,
    Ball = 4
}

class Coordinate
{
    public int X { get; set; }

    public int Y { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Coordinate coordinate &&
               X == coordinate.X &&
               Y == coordinate.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}