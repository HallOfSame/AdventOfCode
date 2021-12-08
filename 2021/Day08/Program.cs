using Day08;
using Helpers;

var inputData = await new SegmentDataReader().ReadInputFromFile();

// Part 1
var part1Digits = new[] { Digit.One, Digit.Four, Digit.Seven, Digit.Eight };
var part1Answer = inputData.SelectMany(x => x.OutputDisplay.Segments)
    .Count(seg => part1Digits.Contains(seg.DisplayedDigit));

Console.WriteLine($"Output with easily recognized digits (1, 4, 7, 8): {part1Answer}");

// Part 2

foreach(var input in inputData)
{

}

class SegmentDataReader : FileReader<DisplayMetadata>
{
    protected override DisplayMetadata ProcessLineOfFile(string line)
    {
        var split = line.Split('|');

        var uniqueSignals = split[0].Split(' ')
            .Select(x => x.ToCharArray().Select(c => Enum.Parse<SignalWire>(c.ToString())).ToArray())
            .Select(x => new Segment(x))
            .ToArray();

        var outputSegments = split[1].Split(' ')
            .Select(x => new Segment(x.ToCharArray().Select(c => Enum.Parse<SignalWire>(c.ToString())).ToArray()))
            .ToArray();

        return new DisplayMetadata
        {
            OutputDisplay = new Display
            {
                Segments = outputSegments
            },
            UniqueSignals = uniqueSignals
        };
    }
}