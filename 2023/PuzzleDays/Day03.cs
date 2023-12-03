using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays;

public class Day03 : ProblemBase
{
    private readonly List<PartNumber> partNumbers = new();
    private Dictionary<Coordinate, char> coordinates;
    private readonly List<Coordinate> gearCandidates = new();
    private readonly List<PartNumber> partNumberCandidates = new();

    protected override async Task<string> SolvePartOneInternal()
    {
        var sum = 0;

        foreach (var candidate in partNumberCandidates)
        {
            var neighborsToCheck = candidate.NumberDigits.SelectMany(x => x.GetNeighbors(true)).Distinct();

            if (neighborsToCheck.Any(x =>
                    coordinates.TryGetValue(x, out var neighborChar) && !char.IsDigit(neighborChar) &&
                    neighborChar != '.'))
            {
                sum += candidate.PartNumberValue;
                partNumbers.Add(candidate);
            }
        }

        return sum.ToString();
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        var gearRatioSum = 0m;

        foreach (var candidate in gearCandidates)
        {
            var gearNeighbors = candidate.GetNeighbors(true)
                .ToHashSet();

            var connectedPartNumbers = partNumbers
                .Where(partNum => partNum.NumberDigits.Any(numCoord => gearNeighbors.Contains(numCoord)))
                .ToList();

            if (connectedPartNumbers.Count != 2)
            {
                continue;
            }

            var thisRatio = connectedPartNumbers[0]
                .PartNumberValue * connectedPartNumbers[1]
                .PartNumberValue;

            gearRatioSum += thisRatio;
        }

        return gearRatioSum.ToString();
    }

    public override async Task ReadInput()
    {
        var strings = await new StringFileReader().ReadInputFromFile();

        var width = strings[0].Length;

        coordinates = new Dictionary<Coordinate, char>();

        var currentDigitString = string.Empty;
        var currentDigitCoordinates = new List<Coordinate>(3);

        for (var y = 0; y < strings.Count; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var value = strings[y][x];

                var coord = new Coordinate(x, y);

                if (value == '*')
                {
                    gearCandidates.Add(coord);
                }

                if (char.IsDigit(value))
                {
                    currentDigitString += value;
                    currentDigitCoordinates.Add(coord);
                }
                else if (currentDigitString.Length != 0)
                {
                    partNumberCandidates.Add(new PartNumber
                    {
                        PartNumberValue = int.Parse(currentDigitString),
                        NumberDigits = currentDigitCoordinates.ToList(),
                    });

                    currentDigitString = string.Empty;
                    currentDigitCoordinates.Clear();
                }

                coordinates.Add(new Coordinate(x, y), value);
            }

            if (currentDigitString.Length == 0)
            {
                continue;
            }

            partNumberCandidates.Add(new PartNumber
            {
                PartNumberValue = int.Parse(currentDigitString),
                NumberDigits = currentDigitCoordinates.ToList(),
            });

            currentDigitString = string.Empty;
            currentDigitCoordinates.Clear();
        }
    }

    private class PartNumber
    {
        public List<Coordinate> NumberDigits { get; set; }

        public int PartNumberValue { get; set; }
    }
}