using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;

var solver = new Solver(new Day25Problem());

await solver.Solve();

class Day25Problem : ProblemBase
{
    protected override async Task<string> SolvePartOneInternal()
    {
        var ocean = new OceanFloor(map);

        var turnsToStop = ocean.MovesUntilMovementStops(false);

        return turnsToStop.ToString();
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        // No part 2, just getting 49 stars
        throw new NotImplementedException();
    }

    public override async Task ReadInput()
    {
        var strings = await new StringFileReader().ReadInputFromFile();

        var tempMap = new List<char[]>();

        foreach (var s in strings)
        {
            var currentRow = s.ToCharArray();

            tempMap.Add(currentRow);
        }

        map = tempMap.To2DArray();
    }

    private char[,] map;
}

class OceanFloor
{
    private readonly char[,] map;

    public OceanFloor(char[,] map)
    {
        this.map = map;
    }

    public int MovesUntilMovementStops(bool draw)
    {
        if (draw)
        {
            map.Draw(x => x.ToString(),
                     "Initial");
        }

        var turnCount = 0;

        while (true)
        {
            var leftRightMoves = ProcessMoves(true);
            var upDownMoves = ProcessMoves(false);

            if (leftRightMoves + upDownMoves == 0)
            {
                break;
            }

            turnCount++;

            if (draw)
            {
                map.Draw(x => x.ToString(),
                         $"Turn {turnCount}");
            }
        }

        return turnCount + 1;
    }

    private int ProcessMoves(bool leftRight)
    {
        var moves = new List<Move>();

        var width = map.GetLength(0);
        var height = map.GetLength(1);

        int GetWrappedX(int x)
        {
            if (x >= width)
            {
                return 0;
            }

            return x;
        }

        int GetWrappedY(int y)
        {
            if (y >= height)
            {
                return 0;
            }

            return y;
        }

        bool CanMoveRight(int x,
                          int y,
                          out Coordinate? target)
        {
            target = new Coordinate(GetWrappedX(x + 1),
                                    y);

            if (map[target.X,
                    target.Y]
                == '.')
            {
                return true;
            }

            return false;
        }

        bool CanMoveDown(int x,
                         int y,
                         out Coordinate? target)
        {
            target = new Coordinate(x,
                                    GetWrappedY(y + 1));

            var targetCurrent = map[target.X,
                                    target.Y];

            if (targetCurrent
                == '.')
            {
                return true;
            }

            return false;
        }


        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var current = map[x,
                                  y];

                if (current == '.')
                {
                    continue;
                }

                if (leftRight && current == '>')
                {
                    if (CanMoveRight(x,
                                     y,
                                     out var targetRight))
                    {
                        moves.Add(new Move
                                  {
                                      Current = new Coordinate(x,
                                                               y),
                                      Target = targetRight,
                                      MovingChar = '>'
                                  });
                    }
                }

                if (!leftRight
                    && current == 'v')
                {
                    if (CanMoveDown(x,
                                    y,
                                    out var targetDown))
                    {
                        moves.Add(new Move
                                  {
                                      Current = new Coordinate(x,
                                                               y),
                                      Target = targetDown,
                                      MovingChar = 'v'
                                  });
                    }
                }
            }
        }

        if (!moves.Any())
        {
            return 0;
        }

        foreach (var move in moves)
        {
            map[move.Target.X,
                move.Target.Y] = move.MovingChar;
            
            map[move.Current.X,
                move.Current.Y] = '.';
        }

        return moves.Count;
    }
}

class Move
{
    public Coordinate Current { get; set; }

    public Coordinate Target { get; set; }

    public char MovingChar { get; set; }
}