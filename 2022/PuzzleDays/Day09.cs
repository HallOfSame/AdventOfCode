using Helpers;
using Helpers.Extensions;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day09 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var rope = new Rope(2);

            foreach (var move in moves)
            {
                rope.ProcessMove(move);
            }

            return rope.TailPositions().ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var rope = new Rope(10);

            foreach (var move in moves)
            {
                rope.ProcessMove(move);
            }

            return rope.TailPositions().ToString();
        }

        public override async Task ReadInput()
        {
            moves = await new MoveReader().ReadInputFromFile();
        }

        private List<Move> moves;
    }
}

public class Rope
{
    private readonly Coordinate[] knots;

    private readonly HashSet<Coordinate> tailPositions;

    public Rope(int knotCount)
    {
        knots = Enumerable.Repeat(0,
                                  knotCount)
                          .Select(_ => new Coordinate(0,
                                                      0))
                          .ToArray();

        tailPositions = new HashSet<Coordinate>();
    }

    public int TailPositions()
    {
        return tailPositions.Count;
    }

    public void ProcessMove(Move move)
    {
        for (var i = 0; i < move.Distance; i++)
        {
            ProcessSingleStep(move.Direction);
        }
    }

    private void ProcessSingleStep(Direction direction)
    {
        var lead = knots[0];

        switch (direction)
        {
            case Direction.North:
                lead.Y += 1;
                break;
            case Direction.South:
                lead.Y -= 1;
                break;
            case Direction.East:
                lead.X += 1;
                break;
            case Direction.West:
                lead.X -= 1;
                break;
        }

        for (var i = 0; i < knots.Length - 1; i++)
        {
            ProcessSingleStepSinglePair(direction,
                                        knots[i],
                                        knots[i + 1]);
        }

        tailPositions.Add(knots[^1]);
    }

    private void ProcessSingleStepSinglePair(Direction direction,
                                             Coordinate lead,
                                             Coordinate follower)
    {
        void UpdateTailX()
        {
            // Update tail up or down
            if (follower.X < lead.X)
            {
                follower.X++;
            }
            else
            {
                follower.X--;
            }
        }

        void UpdateTailY()
        {
            // Update tail left or right
            if (follower.Y < lead.Y)
            {
                follower.Y++;
            }
            else
            {
                follower.Y--;
            }
        }

        var rowDiff = Math.Abs(lead.X - follower.X);
        var columnDiff = Math.Abs(lead.Y - follower.Y);

        if (rowDiff == 0
            && columnDiff == 0)
        {
            return;
        }

        if (rowDiff == 2
            && columnDiff == 0)
        {
            // Same row but too far apart, move left / right
            UpdateTailX();
        }
        else if (rowDiff == 0
                 && columnDiff == 2)
        {
            // Same col too far, move up or down
            UpdateTailY();
        }
        else if (rowDiff + columnDiff == 3 || rowDiff == 2 && columnDiff == 2)
        {
            // Not overlapping or touching diagonally but in different rows and columns
            // Move diagonal
            UpdateTailX();
            UpdateTailY();
        }
    }
}

public class Move
{
    public Direction Direction { get; set; }

    public int Distance { get; set; }
}

public class MoveReader : FileReader<Move>
{
    protected override Move ProcessLineOfFile(string line)
    {
        var split = line.Split(' ');

        var direction = split[0] switch
        {
            "U" => Direction.North,
            "D" => Direction.South,
            "L" => Direction.West,
            "R" => Direction.East,
            _ => throw new ArgumentException(split[0])
        };

        var distance = int.Parse(split[1]);

        return new Move
               {
                   Direction = direction,
                   Distance = distance
               };
    }
}