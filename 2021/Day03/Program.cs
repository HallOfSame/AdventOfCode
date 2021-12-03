using Helpers;

var input = await new CharArrayFileReader().ReadInputFromFile();

// Assuming here all rows are the same length
var singleItemLength = input.First().Length;

var inputByColumns = Enumerable.Range(0, singleItemLength).Select(col => Enumerable.Range(0, input.Count).Select(row => input[row][col]).ToArray()).ToArray();

var gammaRate = string.Empty;
var epsilonRate = string.Empty;

// Part 1

foreach(var column in inputByColumns)
{
    var data = column.GroupBy(x => x).Select(g => new
    {
        Value = g.Key,
        Count = g.Count()
    }).OrderByDescending(x => x.Count).ToList();

    gammaRate += data[0].Value;
    epsilonRate += data[1].Value;
}

Console.WriteLine($"Gamma: {gammaRate}. Epsilon: {epsilonRate}.");

var powerConsumption = Convert.ToInt32(gammaRate, 2) * Convert.ToInt32(epsilonRate, 2);

Console.WriteLine($"Power consumption: {powerConsumption}.");