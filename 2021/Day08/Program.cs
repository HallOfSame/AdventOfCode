using Day08;
using Helpers;
using System.Diagnostics.CodeAnalysis;

var inputData = await new SegmentDataReader().ReadInputFromFile();

// Part 1
var part1Digits = new[] { Digit.One, Digit.Four, Digit.Seven, Digit.Eight };
var part1Answer = inputData.SelectMany(x => x.OutputDisplay.Segments)
    .Count(seg => part1Digits.Contains(seg.DisplayedDigit));

Console.WriteLine($"Output with easily recognized digits (1, 4, 7, 8): {part1Answer}");

// Part 2
var sumDisplay = 0;

foreach(var input in inputData)
{
    var displayValue = Decoder.DecodeDisplay(input);

    sumDisplay += displayValue;
}

Console.WriteLine($"Sum of all displays: {sumDisplay}.");

class Decoder
{
    public static bool Validate(Dictionary<SignalWire, HashSet<SignalWire>> processingMap,
                                Dictionary<SignalWire, SignalWire> finalMap,
                                Segment[] segments,
                                out int displayValue)
    {
        void RemoveFromOptions(SignalWire signalWire,
                       SignalWire[] excludedKeys,
                       Dictionary<SignalWire, HashSet<SignalWire>> mapping)
        {
            mapping.ToList().ForEach(x =>
            {
                if (excludedKeys.Contains(x.Key))
                {
                    return;
                }

                x.Value.Remove(signalWire);
            });
        }

        void AddToFinalMap(SignalWire mixedWire,
                           SignalWire actualWire,
                           Dictionary<SignalWire, HashSet<SignalWire>> pm,
                           Dictionary<SignalWire, SignalWire> fm)
        {
            fm.Add(mixedWire, actualWire);
            pm.Remove(mixedWire);
            RemoveFromOptions(actualWire, Array.Empty<SignalWire>(), pm);
        }

        while (true)
        {
            var determinedOptions = processingMap.Where(x => x.Value.Count == 1).ToList();

            // Only one possiblity left, remove from our in process dictionary and add to the final
            foreach (var option in determinedOptions)
            {
                AddToFinalMap(option.Key, option.Value.First(), processingMap, finalMap);
            }

            if (processingMap.Count == 0)
            {
                // We've determined a fixed mapping
                // Check if it works
                var mappedSegments = segments.Select(x => new Segment(x.EnabledWires.Select(w => finalMap[w]).ToArray(), true)).ToArray();

                if (mappedSegments.All(x => x.DisplayedDigit != Digit.Unknown))
                {
                    displayValue = (((int)mappedSegments[0].DisplayedDigit) * 1000) + (((int)mappedSegments[1].DisplayedDigit) * 100) + (((int)mappedSegments[2].DisplayedDigit) * 10) + (((int)mappedSegments[3].DisplayedDigit) * 1);

                    return true;
                }
                else
                {
                    displayValue = 0;
                    return false;
                }
            }

            // If we have a case where 2 options the same 2 possiblites, we can remove them from any other options
            // We always get at least one of these thanks to the 1 digit
            var matchedPairs = processingMap.Where(x => processingMap.Any(y => x.Key != y.Key && x.Value.SetEquals(y.Value)) && x.Value.Count == 2)
                .GroupBy(x => x.Value, new HashComparer())
                .Select(g => new
                {
                    Values = g.Key.ToArray(),
                    Keys = g.Select(x => x.Key).ToArray()
                })
                .ToList();

            foreach (var pair in matchedPairs)
            {
                foreach (var value in pair.Values)
                {
                    // Remove the matched option from all other possible wires
                    RemoveFromOptions(value, pair.Keys, processingMap);
                }
            }

            if (processingMap.All(x => x.Value.Count == 2))
            {
                // Done all we can, just need to try the options now
                foreach (var pair in matchedPairs)
                {
                    var keyToUse = pair.Keys.First();

                    foreach (var value in pair.Values)
                    {
                        var tempProcessingMap = processingMap.ToDictionary(x => x.Key, x => x.Value.ToHashSet());
                        var tempFinalMap = finalMap.ToDictionary(x => x.Key, x => x.Value);

                        // Lock in this option, then recurse
                        AddToFinalMap(keyToUse, value, tempProcessingMap, tempFinalMap);

                        if (Validate(tempProcessingMap, tempFinalMap, segments, out displayValue))
                        {
                            return true;
                        }
                    }

                    // Was probably an issue with a higher up option
                    displayValue = 0;
                    return false;
                }
            }
        }
    }

    public static int DecodeDisplay(DisplayMetadata displayMetadata)
    {
        // Key is the scrambled wire, value is the possible corrected values
        var signalWireValues = Enum.GetValues<SignalWire>();

        var wireMapping = signalWireValues.ToDictionary(x => x, x => signalWireValues.ToHashSet());

        // Start w/ the easy signals to filter down the need to process
        foreach (var signal in displayMetadata.UniqueSignals.Where(x => x.DisplayedDigit != Digit.Unknown))
        {
            var correctOptions = Segment.CorrectSignals[signal.DisplayedDigit];

            foreach (var enabledWire in signal.EnabledWires)
            {
                wireMapping[enabledWire].IntersectWith(correctOptions);
            }
        }

        // Stores calculated, correct values
        var finalMapping = new Dictionary<SignalWire, SignalWire>();

        if (!Validate(wireMapping, finalMapping, displayMetadata.OutputDisplay.Segments, out var value))
        {
            throw new Exception();
        }

        return value;
    }
}

class HashComparer : IEqualityComparer<HashSet<SignalWire>>
{
    public bool Equals(HashSet<SignalWire>x, HashSet<SignalWire> y)
    {
        return x.SetEquals(y);
    }

    public int GetHashCode([DisallowNull] HashSet<SignalWire> obj)
    {
        return 1;
    }
}

class SegmentDataReader : FileReader<DisplayMetadata>
{
    protected override DisplayMetadata ProcessLineOfFile(string line)
    {
        var split = line.Split(" | ");

        var uniqueSignals = split[0].Split(' ')
            .Select(x => x.ToCharArray().Select(c => Enum.Parse<SignalWire>(c.ToString())).ToArray())
            .Select(x => new Segment(x, false))
            .ToArray();

        var outputSegments = split[1].Split(' ')
            .Select(x => new Segment(x.ToCharArray().Select(c => Enum.Parse<SignalWire>(c.ToString())).ToArray(), false))
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