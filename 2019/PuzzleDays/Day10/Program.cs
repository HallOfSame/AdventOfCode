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
            X = k,
            Y = i
        };

        map.MapSpaces[k, i] = mapSpace;
    }
}

var asteroids = map.Asteroids;

// Part 1

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

// Needed for part 2
MapSpace station = null;

(int x, int y) GetSlopeInt(MapSpace asteroid, MapSpace otherAsteroid)
{
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

    return (slopeX, slopeY);
}

List<MapSpace> GetAsteroidsVisibleFromSpace(MapSpace startSpace,
                                            List<MapSpace> otherAsteroids)
{
    var blockedCoordinates = new HashSet<(int x, int y)>();

    foreach (var otherAsteroid in otherAsteroids)
    {
        if (blockedCoordinates.Contains((otherAsteroid.X, otherAsteroid.Y)))
        {
            continue;
        }

        var (slopeX, slopeY) = GetSlopeInt(startSpace, otherAsteroid);

        var currentX = otherAsteroid.X;
        var currentY = otherAsteroid.Y;

        // Increment by the slope from the other asteroid until we hit the end of the map
        while (true)
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

    var visibleAsteroids = asteroids.Where(x => x != startSpace && !blockedCoordinates.Contains((x.X, x.Y)))
        .ToList();

    return visibleAsteroids;
}

foreach (var asteroid in asteroids)
{
    var totalVisible = GetAsteroidsVisibleFromSpace(asteroid, asteroids.Except(new[] { asteroid }).ToList()).Count;

    if (totalVisible > maxDetectable)
    {
        maxDetectable = totalVisible;
        Console.WriteLine($"Better option found at ({asteroid.X}, {asteroid.Y}) with {totalVisible}.");
        station = asteroid;
    }
}

Console.WriteLine($"Max visible at best option: {maxDetectable}.");

// Part 2
var remainingAsteroids = asteroids.Except(new[]
{
    station
}).ToList();

var vaporizedCount = 0;

const int PuzzleEndVaporizeCount = 200;

while(true)
{
    var visibleAsteroids = GetAsteroidsVisibleFromSpace(station, remainingAsteroids);

    if (vaporizedCount + visibleAsteroids.Count < PuzzleEndVaporizeCount)
    {
        // Don't need to do the extra math to find the order yet
        remainingAsteroids.RemoveAll(x => visibleAsteroids.Contains(x));
    }
    else
    {
        var atanCalc = visibleAsteroids.Select(x =>
        {
            // Ordering by Atan which can tell us the degrees of angle from one point to another
            // Since the laser is pointing straight up, we start at -90 somehow (math man, idk)
            var deltaX = x.X - station.X;
            var deltaY = x.Y - station.Y;
            return new
            {
                // Add 90 so that our first value would have an atan of 0
                atan = (Math.Atan2(deltaY, deltaX) * (180 / Math.PI)) + 90,
                asteroid = x
            };
        })
            // Order negative values to the end, (false comes before true in bool ordering)
            .OrderBy(x => x.atan < 0)
            // Then order ascending, so the list ends up 0, 1, 2, 3, -1, -2, -3, ...
            .ThenBy(x => x.atan)
            .ToList();

        foreach (var calc in atanCalc)
        {
            vaporizedCount++;

            var asteroid = calc.asteroid;

            remainingAsteroids.Remove(asteroid);

            if (vaporizedCount == PuzzleEndVaporizeCount)
            {
                Console.WriteLine($"Asteroid 200: ({asteroid.X}, {asteroid.Y}). Puzzle answer: {(asteroid.X * 100) + asteroid.Y}.");
                break;
            }
        }

    }
}