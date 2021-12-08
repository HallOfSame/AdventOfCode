using Day08;
using Helpers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

var inputData = await new SegmentDataReader().ReadInputFromFile();

// Part 1
var part1Digits = new[] { Digit.One, Digit.Four, Digit.Seven, Digit.Eight };
var part1Answer = inputData.SelectMany(x => x.OutputDisplay.Segments)
    .Count(seg => part1Digits.Contains(seg.DisplayedDigit));

Console.WriteLine($"Output with easily recognized digits (1, 4, 7, 8): {part1Answer}");

// Part 2
var stopwatch = new Stopwatch();

stopwatch.Start();

var sumDisplay = 0;

foreach(var input in inputData)
{
    var displayValue = Decoder.DecodeDisplay(input);

    sumDisplay += displayValue;
}

stopwatch.Stop();

Console.WriteLine($"Sum of all displays: {sumDisplay}. ms: {stopwatch.ElapsedMilliseconds}.");

stopwatch.Restart();

var sumDisplayV2 = 0;

foreach (var input in inputData)
{
    var displayValue = Decoder2.GetDisplayValue(input);

    sumDisplayV2 += displayValue;
}

stopwatch.Stop();

Console.WriteLine($"Sum of all displays v2: {sumDisplayV2}. ms: {stopwatch.ElapsedMilliseconds}.");

class Decoder2
{
    // A much faster way of doing it if you take the time to work out how to use 1,4,7, & 8 to determine which number you are looking at
    public static int GetDisplayValue(DisplayMetadata displayMetadata)
    {
        var map = new Dictionary<int, SignalWire[]>();

        var uniqueSignals = displayMetadata.UniqueSignals;

        IEnumerable<Segment> GetOptionsWithLength(int length)
        {
            return uniqueSignals.Where(x => x.EnabledWires.Length == length);
        }

        // Map the known values based on length
        map[1] = GetOptionsWithLength(2).First().EnabledWires;
        map[4] = GetOptionsWithLength(4).First().EnabledWires;
        map[7] = GetOptionsWithLength(3).First().EnabledWires;
        map[8] = GetOptionsWithLength(7).First().EnabledWires;

        // Then the others
        // We can figure out what they are based on their length and the number of segments they share with digits 4 and 7 (which we already know)
        // 0,6, and 9 are length 6
        map[0] = GetOptionsWithLength(6).RestrictByCommonSegments(3, 3, map).First().EnabledWires;
        map[6] = GetOptionsWithLength(6).RestrictByCommonSegments(3, 2, map).First().EnabledWires;
        map[9] = GetOptionsWithLength(6).RestrictByCommonSegments(4, 3, map).First().EnabledWires;

        // Others are length 5
        map[2] = GetOptionsWithLength(5).RestrictByCommonSegments(2, 2, map).First().EnabledWires;
        map[3] = GetOptionsWithLength(5).RestrictByCommonSegments(3, 3, map).First().EnabledWires;
        map[5] = GetOptionsWithLength(5).RestrictByCommonSegments(3, 2, map).First().EnabledWires;

        int GetDigitForWires(SignalWire[] input)
        {
            return map.Where(x => x.Value.Length == input.Length && x.Value.All(wire => input.Contains(wire))).First().Key;
        }

        // Then just translate that to a string and parse it back as an int
        var outputValue = int.Parse(string.Join("", displayMetadata.OutputDisplay.Segments.Select(x => GetDigitForWires(x.EnabledWires))));

        return outputValue;
    }
}

static class Extensions
{
    public static IEnumerable<Segment> RestrictByCommonSegments(this IEnumerable<Segment> segements, int commonWithFour, int commonWithSeven, Dictionary<int, SignalWire[]> map)
    {
        int GetCommonSegmentsWithDigit(int targetDigit, Segment segment)
        {
            return segment.EnabledWires.Count(wire => map[targetDigit].Contains(wire));
        }

        return segements.Where(x => GetCommonSegmentsWithDigit(4, x) == commonWithFour && GetCommonSegmentsWithDigit(7, x) == commonWithSeven);
    }
}

class Decoder
{
    public static bool Validate(Dictionary<SignalWire, HashSet<SignalWire>> processingMap,
                                Dictionary<SignalWire, SignalWire> finalMap,
                                Segment[] segments,
                                out int displayValue)
    {
        // Removes a passed in wire from the options for wires to map to
        // Can potentially take a list of keys to exlcude during removal
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

        // Marks mixedWire -> actualWire as a final mapping and updates the dictionaries
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

            // Only one possiblity left for this, remove from our in process dictionary and add to the final
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
            // I.e. with the mapping: a -> b | c, d -> b | c, e -> b | c | g we can remove b and c from the possibilities for e.
            // Using b or c for e would result in a or d not having a match
            // This can help find solutions or cut down on the possiblities to check
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
                    // Exlcuding the keys we know have to have one of these values
                    RemoveFromOptions(value, pair.Keys, processingMap);
                }
            }

            // If they all have count 2 then we would loop forever without making progress
            // We've narrowed it down to things like: a and b map to c and d, but we don't know if it goes a->c/b->d or a->d/b->c
            // So we have to start just making a guess and checking
            if (processingMap.All(x => x.Value.Count == 2))
            {
                // Iterate each of the matches up pairs
                foreach (var pair in matchedPairs)
                {
                    // Using the first key in that pair
                    var keyToUse = pair.Keys.First();

                    // Keep trying it with different values
                    foreach (var value in pair.Values)
                    {
                        // Make copies of the current dictionary so we don't alter it permanently
                        var tempProcessingMap = processingMap.ToDictionary(x => x.Key, x => x.Value.ToHashSet());
                        var tempFinalMap = finalMap.ToDictionary(x => x.Key, x => x.Value);

                        // Lock in this option as "final"                        
                        AddToFinalMap(keyToUse, value, tempProcessingMap, tempFinalMap);

                        // Then recurse starting with this new selection
                        // If validate returns true, we found a finalMap setting that creates 4 good digits on output
                        if (Validate(tempProcessingMap, tempFinalMap, segments, out displayValue))
                        {
                            return true;
                        }

                        // Otherwise try the key with the other possible value instead
                    }

                    // If we got to here then we probably recursed multiple levels
                    // And need to break out so that the outer loop will see its guess was wrong
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
            throw new Exception("Could not find valid solution.");
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