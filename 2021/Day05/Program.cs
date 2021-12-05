using Day05;

var lines = await new LineReader().ReadInputFromFile();

var width = lines.SelectMany(x => new int[] { x.Start.X, x.End.X }).Max() + 1;

var heigth = lines.SelectMany(x => new int[] { x.Start.Y, x.End.Y }).Max() + 1;

var field = new Field(width, heigth);

foreach(var line in lines)
{
    if (line.Start.X == line.End.X)
    {
        // Vertical
        for(var i = line.Start.Y; i <= line.End.Y; i++)
        {
            field.FloorData[line.Start.X, i] += 1;
        }
    }
    else if (line.Start.Y == line.End.Y)
    {
        // Horizontal
        for (var i = line.Start.X; i <= line.End.X; i++)
        {
            field.FloorData[i, line.Start.Y] += 1;
        }
    }
    else
    {
        // Comment out for Part 1, where we ignore diagonals
        var yMult = line.Start.Y > line.End.Y ? -1 : 1;
        var xMult = line.Start.X > line.End.X ? -1 : 1;

        var lineLength = Math.Abs(line.Start.X - line.End.X);

        for(var i = 0; i <= lineLength; i++)
        {
            field.FloorData[line.Start.X + (i * xMult), line.Start.Y + (i * yMult)] += 1;
        }
    }
}

// Kinda useless on real input
//Console.WriteLine(field.ToString());

var floorTilesFlattened = field.FloorData.Cast<int>();

var numPointsAtLeastTwo = floorTilesFlattened.Count(x => x >= 2);

Console.WriteLine($"Sections with value 2 or more: {numPointsAtLeastTwo}.");