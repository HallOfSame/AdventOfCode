using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays;

public class Day03 : ProblemBase
{
    private readonly List<PartNumber> partNumbers = new();
    private Dictionary<Coordinate, char> coordinates;

    protected override async Task<string> SolvePartOneInternal()
    {
        var startOfPartNumbers = coordinates.Where(x =>
        {
            var isDigit = char.IsDigit(x.Value);

            if (!isDigit)
            {
                return false;
            }

            var leftSpace = x.Key.X - 1;

            if (leftSpace < 0)
            {
                // Is a digit and nothing to the left
                return true;
            }

            return !char.IsDigit(coordinates[new Coordinate(leftSpace, x.Key.Y)]);
        }).ToList();

        var partNumberCandidates = new List<PartNumber>();

        foreach (var startOfPartNumber in startOfPartNumbers)
        {
            var partNumber = "" + startOfPartNumber.Value;

            var currentCoordinate = startOfPartNumber.Key;

            var partCoordinates = new List<Coordinate>
            {
                currentCoordinate
            };

            do
            {
                var nextCoordinate = new Coordinate(currentCoordinate.X + 1, currentCoordinate.Y);

                if (!coordinates.TryGetValue(nextCoordinate, out var nextChar))
                {
                    break;
                }

                if (!char.IsDigit(nextChar))
                {
                    break;
                }

                partNumber += nextChar;
                partCoordinates.Add(nextCoordinate);
                currentCoordinate = nextCoordinate;
            } while (true);

            partNumberCandidates.Add(new PartNumber
            {
                NumberDigits = partCoordinates,
                PartNumberValue = int.Parse(partNumber)
            });
        }

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
        var gearCandidates = coordinates.Where(x => x.Value == '*');

        var gearRatioSum = 0m;

        foreach (var candidate in gearCandidates)
        {
            var gearNeighbors = candidate.Key.GetNeighbors(true)
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

        for (var x = 0; x < width; x++)
        for (var y = 0; y < strings.Count; y++)
        {
            var value = strings[y][x];

            coordinates.Add(new Coordinate(x, y), value);
        }
    }

    private class PartNumber
    {
        public List<Coordinate> NumberDigits { get; set; }

        public int PartNumberValue { get; set; }
    }
}