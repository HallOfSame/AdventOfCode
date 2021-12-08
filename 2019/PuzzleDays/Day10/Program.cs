using Day10;
using Helpers;

var input = await new MultilineStringReader().ReadInputFromFile();

var mapHeight = input.Count;
var mapWidth = input[0].Length;

// Create and load the map
var map = new Map(mapHeight, mapWidth);

for(var i = 0; i < input.Count; i++)
{
    var row = input[i];
    var splitRow = row.ToCharArray();

    for(var k = 0; k < splitRow.Length; k++)
    {
        var space = splitRow[k];

        var mapSpace = new MapSpace
        {
            HasAsteroid = space == '#',
            X = i,
            Y = k
        };

        map.MapSpaces[i, k] = mapSpace;
    }
}

var asteroids = map.Asteroids;

var maxDetectable = 0;

int GCD(int a, int b)
{
    while (a != 0 && b != 0)
    {
        if (a > b)
        {
            a %= b;
        }
        else
        {
            b %= a;
        }
    }

    return a | b;
}

foreach (var asteroid in asteroids)
{
    var blockedCoordinates = new HashSet<(int x, int y)>();

    foreach(var otherAsteroid in asteroids)
    {
        if (asteroid == otherAsteroid)
        {
            continue;
        }

        if (blockedCoordinates.Contains((otherAsteroid.X, otherAsteroid.Y)))
        {
            continue;
        }

        var slopeX = otherAsteroid.X - asteroid.X;
        var slopeY = otherAsteroid.Y - asteroid.Y;

        // Simplify the slope
        if (slopeY == 0)
        {
            // Direct X line
            slopeX = slopeX > 0 ? 1 : -1;
        }
        else if (slopeX == 0)
        {
            // Direct Y line
            slopeY = slopeY > 0 ? 1 : -1;
        }
        else
        {
            // Check GCD (implementation needs positive values)
            var gcd = GCD(Math.Abs(slopeY), Math.Abs(slopeX));

            // If it wasn't 1, divide both by the returned value to get the simplified slope
            // Otherwise we can miss blocked coordinates along the path
            if (gcd != 1)
            {
                slopeY /= gcd;
                slopeX /= gcd;
            }
        }

        var currentX = otherAsteroid.X;
        var currentY = otherAsteroid.Y;

        // Increment by the slope from the other asteroid until we hit the end of the map
        while(true)
        {
            var nextCoordinateInLine = (x: currentX + slopeX, y: currentY + slopeY);

            if (!map.IsInBounds(nextCoordinateInLine.x, nextCoordinateInLine.y))
            {
                break;
            }

            blockedCoordinates.Add(nextCoordinateInLine);

            currentX = nextCoordinateInLine.x;
            currentY = nextCoordinateInLine.y;
        }
    }

    var totalVisible = asteroids.Where(x => x != asteroid && !blockedCoordinates.Contains((x.X, x.Y)))
        .Count();

    if (totalVisible > maxDetectable)
    {
        maxDetectable = totalVisible;
        Console.WriteLine($"Better option found at ({asteroid.X}, {asteroid.Y}) with {totalVisible}.");
    }
}

Console.WriteLine($"Max visible at best option: {maxDetectable}.");