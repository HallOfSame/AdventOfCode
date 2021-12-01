using Helpers;

var fileLines = await new IntFileReader().ReadInputFromFile();

// Part 1
var increases = 0;

for (var i = 1; i < fileLines.Count; i++)
{
    if (fileLines[i - 1] < fileLines[i])
    {
        increases++;
    }
}

Console.WriteLine($"Part 1: {increases}.");

// Part 2
increases = 0;

var previousWindow = fileLines[0] + fileLines[1] + fileLines[2];

for (var i = 3; i < fileLines.Count; i++)
{
    var currentWindow = fileLines[i - 2] + fileLines[i - 1] + fileLines[i];

    if (previousWindow < currentWindow)
    {
        increases++;
    }

    previousWindow = currentWindow;
}

Console.WriteLine($"Part 2: {increases}.");