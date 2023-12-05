using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.FileReaders;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day05 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var locationMin = long.MaxValue;

            foreach (var seed in startingSeeds)
            {
                var currentValue = seed;

                // Goal is location, but the puzzle input goes in order so we can ignore the names and just iterate the list
                foreach (var converter in converters)
                {
                    var convertedValue = converter.ConvertValue(currentValue);

                    currentValue = convertedValue;
                }

                locationMin = Math.Min(locationMin, currentValue);
            }

            return locationMin.ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var seedRanges = new List<Range>();

            // Build the seed ranges
            for (var i = 0; i < startingSeeds.Count; i += 2)
            {
                var rangeStart = startingSeeds[i];
                var rangeLength = startingSeeds[i + 1];

                seedRanges.Add(new Range
                {
                    Start = rangeStart,
                    Length = rangeLength,
                });
            }

            var ranges = seedRanges;

            // Iterate the conversion stages
            foreach (var converter in converters)
            {
                // Break apart the ranges at this stage to all the ranges the could be in the next stage
                ranges = ranges.SelectMany(x => ConvertRange(x, converter)).ToList();
            }

            // Then just find the smallest start of a range that we have available at the end
            return ranges.Min(x => x.Start).ToString();
        }

        private List<Range> ConvertRange(Range initialRange, Converter converter)
        {
            var minValue = initialRange.Start;

            var maxValue = initialRange.End;

            var splitRanges = new List<Range>();

            // Taken from S/O, returns the length of overlap
            long FindOverlap(Range one, Range two)
            {
                return Math.Max(0, Math.Min(one.End, two.End) - Math.Max(one.Start, two.Start) + 1);
            }

            // First, search the mapping table for the next stage
            // Find any parts of the initial range that get mapped to different values in the new stage
            foreach (var map in converter.RangeMaps)
            {
                // If this doesn't overlap the input range, just ignore it
                if (FindOverlap(map.SourceRange, initialRange) == 0)
                {
                    continue;
                }

                // Calculate the split out from the original range that overlaps this mapped range
                var newRangeStart = Math.Max(minValue, map.SourceRange.Start);
                var newRangeEnd = Math.Min(maxValue, map.SourceRange.End);

                splitRanges.Add(new Range
                {
                    Start = newRangeStart,
                    Length = newRangeEnd - newRangeStart + 1,
                });
            }

            // Put the splits in ascending order so that we can fill in the gaps
            splitRanges = splitRanges.OrderBy(x => x.Start)
                .ToList();

            // If no splits, nothing overlaps and we can return the initial range
            if (!splitRanges.Any())
            {
                return new List<Range>
                {
                    initialRange
                };
            }

            var currentValue = initialRange.Start;

            var splitIndex = 0;

            // Tracking pieces from the input range that don't have a mapping and just use the same value in the destination
            var untouchedRanges = new List<Range>();

            while (true)
            {
                // If our current value is below the next split
                if (currentValue - splitRanges[splitIndex]
                        .Start != 0)
                {
                    // Add the gap between where we are and our next split
                    untouchedRanges.Add(new Range
                    {
                        Start = currentValue,
                        Length = splitRanges[splitIndex]
                            .Start - currentValue,
                    });
                }

                // Move to the end of the next split
                currentValue = splitRanges[splitIndex]
                    .End + 1;
                splitIndex++;

                // Stop when we run out of splits
                if (splitIndex == splitRanges.Count)
                {
                    break;
                }
            }

            // Add the end if we have any gap remaining
            if (currentValue < maxValue)
            {
                untouchedRanges.Add(new Range
                {
                    Start = currentValue,
                    Length = maxValue - currentValue + 1,
                });
            }

            // Now we've split out the sections of the initial range that get mapped to new values
            // And we've calculated the ranges to fill the gaps in the initial range that don't get mapped

            // Convert the mapped ranges to the new values in the destination
            var mappedRanges = splitRanges.Select(x => new Range
            {
                Start = converter.ConvertValue(x.Start),
                Length = x.Length,
            });

            // Then concat everything and return
            return untouchedRanges.Concat(mappedRanges)
                .ToList();
        }

        public override async Task ReadInput()
        {
            var strings = await new StringFileReader().ReadInputFromFile();

            var seedsLine = strings[0];

            startingSeeds = seedsLine.Split(':')[1]
                .Trim()
                .Split()
                .Select(long.Parse)
                .ToList();

            converters = new List<Converter>();

            // Start at 2 to skip the blank line
            for (var i = 2; i < strings.Count; i++)
            {
                if (string.IsNullOrEmpty(strings[i]))
                {
                    continue;
                }

                var descriptionLine = strings[i].Split("-to-");

                var source = descriptionLine[0]
                    .Trim();

                var destination = descriptionLine[1]
                    .Split()[0];

                var rangeMaps = new List<RangeMap>();

                do
                {
                    i += 1;

                    var rangeValues = strings[i]
                        .Split()
                        .Select(long.Parse)
                        .ToArray();

                    var destinationStart = rangeValues[0];
                    var sourceStart = rangeValues[1];
                    var rangeLength = rangeValues[2];

                    var sourceRange = new Range
                    {
                        Start = sourceStart,
                        Length = rangeLength,
                    };

                    var destinationRange = new Range
                    {
                        Start = destinationStart,
                        Length = rangeLength,
                    };

                    var rangeMap = new RangeMap
                    {
                        SourceRange = sourceRange,
                        DestinationRange = destinationRange,
                    };

                    rangeMaps.Add(rangeMap);

                } while (i + 1 < strings.Count && !string.IsNullOrWhiteSpace(strings[i + 1]));

                converters.Add(new Converter
                {
                    SourceName = source,
                    DestinationName = destination,
                    RangeMaps = rangeMaps,
                });
            }
        }

        private List<long> startingSeeds;

        private List<Converter> converters;

        class Converter
        {
            public string SourceName { get; set; }

            public string DestinationName { get; set; }

            public List<RangeMap> RangeMaps { get; set; }

            public long ConvertValue(long value)
            {
                var matchingRange = RangeMaps.FirstOrDefault(x => x.SourceRange.IsNumberWithinRange(value));

                if (matchingRange is null)
                {
                    return value;
                }

                return matchingRange.ConvertToDestinationValue(value);
            }

            public long InverseConvertValue(long value)
            {
                var matchingRange = RangeMaps.FirstOrDefault(x => x.DestinationRange.IsNumberWithinRange(value));

                if (matchingRange is null)
                {
                    return value;
                }

                return matchingRange.ConvertToSourceValue(value);
            }
        }

        [DebuggerDisplay("{SourceRange} {DestinationRange}")]
        class RangeMap
        {
            public Range SourceRange { get; set; }

            public Range DestinationRange { get; set; }

            public long ConvertToDestinationValue(long value)
            {
                return SourceRange.GetRangeOffset(value) + DestinationRange.Start;
            }

            public long ConvertToSourceValue(long value)
            {
                return DestinationRange.GetRangeOffset(value) + SourceRange.Start;
            }
        }

        [DebuggerDisplay("{Start} - {End} ({Length})")]
        class Range
        {
            public long Start { get; set; }

            public long Length { get; set; }

            public long End
            {
                get { return Start + Length - 1; }
            }

            public bool IsNumberWithinRange(long testValue)
            {
                return testValue >= Start && testValue <= End;
            }

            public long GetRangeOffset(long value)
            {
                return value - Start;
            }
        }
    }
}