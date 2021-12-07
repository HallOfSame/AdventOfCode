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

// Part 2

// Not sure we really need all this
// I didn't test just calculating it each time -> Enumerable.Range(0, difference).Sum()
// I tested it just now, didn't wait around for it to finish
var fuelRequirements = new Dictionary<int, int>();

int FuelRequired(int currentPosition, int destinationPosition)
{
    var difference = Math.Abs(currentPosition - destinationPosition);

    if (difference == 1)
    {
        return 1;
    }

    if (fuelRequirements.ContainsKey(difference))
    {
        return fuelRequirements[difference];
    }

    var positionMove = currentPosition > destinationPosition ? -1 : 1;

    var requiredFuel = FuelRequired(currentPosition + positionMove, destinationPosition) + difference;

    fuelRequirements[difference] = requiredFuel;

    return requiredFuel;
}

var bestOptionPart2 = Enumerable.Range(minPos, maxPos - minPos)
    .Select(testPos => new
    {
        FuelRequired = horizontalPositions.Select(x => FuelRequired(x, testPos)).Sum(),
        Position = testPos
    })
    .OrderBy(x => x.FuelRequired)
    .First();

Console.WriteLine($"Best position with scaling fuel is {bestOptionPart2.Position} requiring {bestOptionPart2.FuelRequired} fuel.");