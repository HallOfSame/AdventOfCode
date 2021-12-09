/*
 * 2199943210
3987894921
9856789892
8767896789
9899965678
*/

using Helpers;
using Helpers.Structure;
using System.Text;

var solver = new Solver(new Day9Problem());

await solver.Solve();

class Day9Problem : ProblemBase
{
    private Map map;

    private MapPoint[] lowPoints;

    public override async Task ReadInput()
    {
        var points = await new MapPointReader().ReadInputFromFile();

        var points2dArray = new MapPoint[points[0].Length, points.Count];

        points.SelectMany(x => x).ToList().ForEach(x => points2dArray[x.X, x.Y] = x);

        map = new Map(points2dArray);
    }

    protected override Task<string> SolvePartOneInternal()
    {
        lowPoints = map.Points.Cast<MapPoint>().Where(x => map.IsLowPoint(x)).ToArray();

        var lowPointRiskLevels = lowPoints.Select(x => x.RiskLevel).Sum().ToString();

        return Task.FromResult(lowPointRiskLevels);
    }

    protected override Task<string> SolvePartTwoInternal()
    {
        var basinSizes = lowPoints.Select(x => map.GetBasinSize(x)).OrderByDescending(x => x).ToArray();

        var partTwoAnswer = basinSizes.Take(3).Aggregate((curr, next) => curr *= next);

        return Task.FromResult(partTwoAnswer.ToString());
    }
}

class MapPointReader : FileReader<MapPoint[]>
{
    private int CurrentRow = 0;

    protected override MapPoint[] ProcessLineOfFile(string line)
    {
        var pointHeights = line.Select(x => new string(x, 1))
            .Select(x => int.Parse(x))
            .Select((height, idx) => new MapPoint
            {
                X = idx,
                Y = CurrentRow, 
                Height = height
            })
            .ToArray();

        CurrentRow++;

        return pointHeights;
    }
}

enum Direction
{
    Up,
    Down,
    Left,
    Right
}

class Map
{
    public Map(MapPoint[,] points)
    {
        Points = points;
        Height = Points.GetLength(1);
        Width = Points.GetLength(0);
    }

    public MapPoint[,] Points { get; }

    public int Height { get; }

    public int Width { get; }

    private bool IsValidX(int x)
    {
        return x >= 0 && x < Width;
    }

    private bool IsValidY(int y)
    {
        return y >= 0 && y < Height;
    }

    private MapPoint? GetPointInDirection(Direction direction, MapPoint currentPoint)
    {
        var x = currentPoint.X;
        var y = currentPoint.Y;

        switch(direction)
        {
            case Direction.Up:
                y -= 1;
                break;
                case Direction.Down:
                y += 1;
                break;
            case Direction.Left:
                x -= 1;
                break;
            case Direction.Right:
                x += 1;
                break;
        }

        if (!IsValidX(x) || !IsValidY(y))
        {
            return null;
        }

        return Points[x, y];
    }

    private bool DoesBasinContinue(MapPoint nextPoint, MapPoint currentBasinPoint)
    {
        return nextPoint.Height != 9 && nextPoint.Height > currentBasinPoint.Height;
    }

    public int GetBasinSize(MapPoint lowPoint)
    {
        var allDirections = Enum.GetValues<Direction>();

        // Holds the points we are expanding from
        var currentPointsToEvaluate = new[] { lowPoint };

        // Holds all points marked as being in the current basin
        var pointsInBasin = new HashSet<MapPoint>
        {
            lowPoint
        };

        while (true)
        {
            currentPointsToEvaluate = currentPointsToEvaluate.Select(x => new
                {
                    // Grab the points in each direction from our current point, provided they are valid and not already in the basin
                    currentPoint = x,
                    otherDirections = allDirections.Select(dir => GetPointInDirection(dir, x)).Where(x => x != null && !pointsInBasin.Contains(x)).ToArray()
                })
                // From them, get the points where the basin continues on
                .SelectMany(x => x.otherDirections.Where(newPoint => DoesBasinContinue(newPoint, x.currentPoint)))
                .ToArray();

            // We've exhausted the search in all directions
            if (!currentPointsToEvaluate.Any())
            {
                break;
            }

            // Add the points to our hash set before looping
            foreach(var newPoint in currentPointsToEvaluate)
            {
                pointsInBasin.Add(newPoint);
            }
        }

        return pointsInBasin.Count;
    }

    public bool IsLowPoint(MapPoint pointToCheck)
    {
        var up = GetPointInDirection(Direction.Up, pointToCheck);
        var down = GetPointInDirection(Direction.Down, pointToCheck);
        var left = GetPointInDirection(Direction.Left, pointToCheck);
        var right = GetPointInDirection(Direction.Right, pointToCheck);

        if (up != null && up.Height <= pointToCheck.Height)
        {
            return false;
        }

        if (down != null && down.Height <= pointToCheck.Height)
        {
            return false;
        }

        if (left != null && left.Height <= pointToCheck.Height)
        {
            return false;
        }

        if (right != null && right.Height <= pointToCheck.Height)
        {
            return false;
        }

        return true;
    }

    public void DrawMap()
    {
        var stringBuilder = new StringBuilder();

        for(var y = 0; y < Height; y++)
        {
            var currentLine = string.Empty;

            for(var x = 0; x < Width; x++)
            {
                currentLine += Points[x, y].Height;
            }

            stringBuilder.AppendLine(currentLine);
        }

        Console.WriteLine(stringBuilder.ToString());
    }
}

class MapPoint
{
    public int X { get; set; }

    public int Y { get; set; }

    public int Height { get; set; }

    public int RiskLevel => Height + 1;

    public override bool Equals(object? obj)
    {
        return obj is MapPoint point &&
               X == point.X &&
               Y == point.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}