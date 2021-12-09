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

    public override async Task ReadInput()
    {
        var points = await new MapPointReader().ReadInputFromFile();

        var points2dArray = new MapPoint[points[0].Length, points.Count];

        points.SelectMany(x => x).ToList().ForEach(x => points2dArray[x.X, x.Y] = x);

        map = new Map(points2dArray);
    }

    protected override Task<string> SolvePartOneInternal()
    {
        var lowPointRiskLevels = map.Points.Cast<MapPoint>().Where(x => map.IsLowPoint(x)).Select(x => x.RiskLevel).Sum().ToString();

        return Task.FromResult(lowPointRiskLevels);
    }

    protected override Task<string> SolvePartTwoInternal()
    {
        throw new NotImplementedException();
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

    public bool IsLowPoint(MapPoint pointToCheck)
    {
        var currentPointX = pointToCheck.X;
        var currentPointY = pointToCheck.Y;

        var upY = currentPointY - 1;
        var downY = currentPointY + 1;
        var leftX = currentPointX - 1;
        var rightX = currentPointX + 1;

        if (IsValidX(leftX) && Points[leftX, currentPointY].Height <= pointToCheck.Height)
        {
            return false;
        }

        if (IsValidX(rightX) && Points[rightX, currentPointY].Height <= pointToCheck.Height)
        {
            return false;
        }

        if (IsValidY(upY) && Points[currentPointX, upY].Height <= pointToCheck.Height)
        {
            return false;
        }

        if (IsValidY(downY) && Points[currentPointX, downY].Height <= pointToCheck.Height)
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
}