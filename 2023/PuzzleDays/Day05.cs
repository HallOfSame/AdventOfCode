using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
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
        }

        class RangeMap
        {
            public Range SourceRange { get; set; }

            public Range DestinationRange { get; set; }

            public long ConvertToDestinationValue(long value)
            {
                return SourceRange.GetRangeOffset(value) + DestinationRange.Start;
            }
        }

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