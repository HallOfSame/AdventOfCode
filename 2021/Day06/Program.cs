using Helpers.FileReaders;

var numbers = (await new StringFileReader().ReadInputFromFile())
    .SelectMany(x => x.Split(','))
    .Select(x => int.Parse(x))
    .ToList();

// Part 1
const int NormalCycleStart = 6;
const int NewFishCycleStart = NormalCycleStart + 2;
const int PartOneDays = 80;

void RunFishLifeSimulation(int numberOfDays)
{
    var fishLives = numbers.GroupBy(x => x)
        .ToDictionary(g => g.Key, g => (long)g.Count());

    foreach (var day in Enumerable.Range(0, numberOfDays))
    {
        // Grab the 0 count
        fishLives.TryGetValue(0, out var newFishThisDay);

        // Evaluate life times
        for(var i = 1; i < 9; i++)
        {
            // Handle the missing keys for the first few days
            var newCount = fishLives.TryGetValue(i, out var dictCount) ? dictCount : 0;

            fishLives[i - 1] = newCount;
        }

        // Process the fish that created new ones this cycle
        fishLives[NormalCycleStart] += newFishThisDay;

        // Add the new fish for this cycle
        fishLives[NewFishCycleStart] = newFishThisDay;

        //var newFish = numbers.Count(x => x == 0);

        //numbers = numbers.Select(x => x == 0 ? NormalCycleStart : x - 1)
        //    .Concat(Enumerable.Repeat(NewFishCycleStart, newFishThisDay))
        //    .ToList();
    }

    var numberOfFish = fishLives.Select(x => x.Value).Sum();

    Console.WriteLine($"Number of fish after {numberOfDays} days: {numberOfFish}.");
}

RunFishLifeSimulation(PartOneDays);

// Part 2
const int PartTwoDays = 256;

RunFishLifeSimulation(PartTwoDays);