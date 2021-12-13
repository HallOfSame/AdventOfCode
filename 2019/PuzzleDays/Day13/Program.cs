using IntCodeInterpreter;
using IntCodeInterpreter.Input;

var gameCode = await new FileInputParser().ReadOperationsFromFile("PuzzleInput.txt");

var game = new ArcadeGame(gameCode);

game.Run();

class ArcadeGame
{
    private readonly List<long> gameCode;

    private Interpreter interpreter;

    public ArcadeGame(List<long> gameCode)
    {
        this.gameCode = gameCode;
        interpreter = new Interpreter();
        gameMap = new HashSet<GameTile>();
    }

    public void Run()
    {
        var outputBuffer = new List<long>(3);

        interpreter.ProcessOperations(gameCode,
                                      () =>
                                      {
                                          throw new NotImplementedException("Input not implemented.");
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

        Console.WriteLine($"Number of block tiles: {gameMap.Count(x => x.Type == TileType.Block)}.");
    }

    private void ProcessOutput(List<long> outputData)
    {
        var coordinate = new Coordinate
                         {
                             X = (int)outputData[0],
                             Y = (int)outputData[1]
                         };

        var tile = new GameTile
                   {
                       Position = coordinate,
                       Type = (TileType)(int)outputData[2]
                   };

        if (gameMap.Contains(tile))
        {
            throw new NotImplementedException("Updating existing tile not implemented.");
        }

        gameMap.Add(tile);
    }

    private HashSet<GameTile> gameMap;
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