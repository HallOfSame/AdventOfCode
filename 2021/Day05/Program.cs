using Day05;

var lines = await new LineReader().ReadInputFromFile();

var width = lines.SelectMany(x => new int[] { x.Start.X, x.End.X }).Max() + 1;

var heigth = lines.SelectMany(x => new int[] { x.Start.Y, x.End.Y }).Max() + 1;

var field = new Field(width, heigth);

// Part 1

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
        // Part 1, ignore diagonals
        //throw new InvalidDataException("Did not expect diagonal line.");
    }
}

Console.WriteLine(field.ToString());

var floorTilesFlattened = field.FloorData.Cast<int>();

var numPointsAtLeastTwo = floorTilesFlattened.Count(x => x >= 2);

Console.WriteLine($"Sections with value 2 or more: {numPointsAtLeastTwo}.");