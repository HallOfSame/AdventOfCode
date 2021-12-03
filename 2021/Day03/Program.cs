using Helpers;

var input = await new CharArrayFileReader().ReadInputFromFile();

// Assuming here all rows are the same length
var singleItemLength = input.First().Length;

char[][] FlipToColumnBasedArray(List<char[]> originalArray)
{
    // Create a new list with one item per character in the binary strings
    // Each item is an array with the column values from every row in the original input at that index
    // So the output[2] is an array of the 3rd column from every input row. And output[2][3] is the 3rd column of the 4th row in the input.
    return Enumerable.Range(0, singleItemLength).Select(col => Enumerable.Range(0, originalArray.Count).Select(row => originalArray[row][col]).ToArray()).ToArray();
}

var inputByColumns = FlipToColumnBasedArray(input);

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

// Part 2

var oxygenOptions = input.ToList();
var co2Options = input.ToList();

var oxygenSetting = string.Empty;
var co2Setting = string.Empty;

for (var columnIndex = 0; columnIndex < inputByColumns.Length; columnIndex++)
{
    char GetFilterForIndex(bool isOxygen)
    {
        var arrayToProcess = isOxygen ? oxygenOptions : co2Options;

        var column = FlipToColumnBasedArray(arrayToProcess)[columnIndex];

        var data = column.GroupBy(x => x).Select(g => new
        {
            Value = g.Key,
            Count = g.Count()
        }).OrderByDescending(x => x.Count).ToList();

        if (data[0].Count == data[1].Count)
        {
            return isOxygen ? '1' : '0';
        }

        return isOxygen ? data[0].Value : data[1].Value;
    }    

     
    var stillProcessing = false;

    if (string.IsNullOrEmpty(oxygenSetting))
    {
        var oxygenFilter = GetFilterForIndex(true);

        oxygenOptions = oxygenOptions.Where(x => x[columnIndex] == oxygenFilter).ToList();

        if (oxygenOptions.Count == 1)
        {
            oxygenSetting = new string(oxygenOptions.First());
            stillProcessing = false;
        }
        else
        {
            stillProcessing = true;
        }
    }

    if (string.IsNullOrEmpty(co2Setting))
    {
        var co2Filter = GetFilterForIndex(false);

        co2Options = co2Options.Where(x => x[columnIndex] == co2Filter).ToList();

        if (co2Options.Count == 1)
        {
            co2Setting = new string(co2Options.First());
        }
        else
        {
            stillProcessing = true;
        }
    }

    if (!stillProcessing)
    {
        break;
    }
}

Console.WriteLine($"Oxy: {oxygenSetting}. CO2: {co2Setting}.");

var lifeSupport = Convert.ToInt32(oxygenSetting, 2) * Convert.ToInt32(co2Setting, 2);

Console.WriteLine($"Life support: {lifeSupport}.");