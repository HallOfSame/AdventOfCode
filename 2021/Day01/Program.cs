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

Console.WriteLine(increases);