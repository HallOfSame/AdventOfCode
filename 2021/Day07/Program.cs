using Helpers.FileReaders;

var horizontalPositions = (await new StringFileReader().ReadInputFromFile()).SelectMany(x => x.Split(',')).Select(x => int.Parse(x)).ToList();

var minPos = horizontalPositions.Min();
var maxPos = horizontalPositions.Max();

// Part 1

var bestOption = Enumerable.Range(minPos, maxPos - minPos)
    .Select(testPos => new
    {
        FuelRequired = horizontalPositions.Select(x => Math.Abs(x - testPos)).Sum(),
        Position = testPos
    })
    .OrderBy(x => x.FuelRequired)
    .First();

Console.WriteLine($"Best position is {bestOption.Position} requiring {bestOption.FuelRequired} fuel.");