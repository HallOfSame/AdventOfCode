using Helpers.FileReaders;

var numbers = (await new StringFileReader().ReadInputFromFile())
    .SelectMany(x => x.Split(','))
    .Select(x => int.Parse(x))
    .ToList();

// Part 1
const int NormalCycleStart = 6;
const int NewFishCycleStart = NormalCycleStart + 2;
const int NumberOfDays = 80;

foreach(var day in Enumerable.Range(0, NumberOfDays))
{
    var newFish = numbers.Count(x => x == 0);

    numbers = numbers.Select(x => x == 0 ? NormalCycleStart : x - 1)
        .Concat(Enumerable.Repeat(NewFishCycleStart, newFish))
        .ToList();
}

Console.WriteLine($"Number of fish after {NumberOfDays} days: {numbers.Count}.");