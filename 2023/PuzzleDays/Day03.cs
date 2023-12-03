using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays;

public class Day03 : ProblemBase
{
    private Dictionary<Coordinate, char> coordinates;

    protected override async Task<string> SolvePartOneInternal()
    {
        var startOfPartNumbers = coordinates.Where(x =>
        {
            var isDigit = char.IsDigit(x.Value);

            if (!isDigit) return false;

            var leftSpace = x.Key.X - 1;

            if (leftSpace < 0)
                // Is a digit and nothing to the left
                return true;

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

                if (!coordinates.TryGetValue(nextCoordinate, out var nextChar)) break;

                if (!char.IsDigit(nextChar)) break;

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
                sum += candidate.PartNumberValue;
        }

        return sum.ToString();
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        throw new NotImplementedException();
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