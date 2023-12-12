using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using Helpers.Extensions;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day12 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var result = records.Select(CalculateArrangements)
                .Sum();

            return result.ToString();
        }

        private decimal CalculateArrangements(Record record)
        {
            return CalculateRecursive(record.DamagedRecord, 0, record, new Dictionary<(int, int, int, int), decimal>());
        }

        private decimal CalculateRecursive(string partialString, int currentIndex, Record record, Dictionary<(int, int, int, int), decimal> knownPartials)
        {
            if (!IsValid(partialString, record, out var currentGroupSize, out var groupCount))
            {
                return 0m;
            }

            if (currentIndex == partialString.Length)
            {
                // Base case
                return 1m;
            }

            if (partialString[currentIndex] != '?')
            {
                return CalculateRecursive(partialString, currentIndex + 1, record, knownPartials);
            }

            var inGroup = currentIndex > 0 && partialString[currentIndex - 1] == '#';

            if (knownPartials.TryGetValue((currentIndex, currentGroupSize, groupCount, inGroup ? 0 : 1), out var knownTotal))
            {
                return knownTotal;
            }

            var usingDot = partialString.ReplaceAt(currentIndex, '.');
            var usingHash = partialString.ReplaceAt(currentIndex, '#');
            var total = 0m;

            total += CalculateRecursive(usingDot, currentIndex + 1, record, knownPartials);

            total += CalculateRecursive(usingHash, currentIndex + 1, record, knownPartials);

            knownPartials[(currentIndex, currentGroupSize, groupCount, inGroup ? 0 : 1)] = total;

            return total;
        }

        private bool IsValid(string proposedRestoration, Record record, out int currentGroupSize, out int currentIndex)
        {
            currentIndex = 0;
            var currentGroupSizeLimit = record.DamagedGroupsCount[currentIndex];
            currentGroupSize = 0;
            var failOnHash = false;

            for (var i = 0; i < proposedRestoration.Length; i++)
            {
                if (proposedRestoration[i] == '?')
                {
                    return true;
                }

                if (proposedRestoration[i] == '#')
                {
                    if (failOnHash)
                    {
                        return false;
                    }

                    currentGroupSize++;

                    if (currentGroupSize > currentGroupSizeLimit)
                    {
                        return false;
                    }
                }

                if (proposedRestoration[i] == '.' && currentGroupSize > 0)
                {
                    if (currentGroupSize != currentGroupSizeLimit)
                    {
                        return false;
                    }

                    currentGroupSize = 0;
                    currentIndex++;

                    if (currentIndex >= record.DamagedGroupsCount.Count)
                    {
                        failOnHash = true;
                    }
                    else
                    {
                        currentGroupSizeLimit = record.DamagedGroupsCount[currentIndex];
                    }
                }
            }

            if (currentGroupSize > 0 && currentGroupSize != currentGroupSizeLimit)
            {
                return false;
            }

            if (currentGroupSize > 0)
            {
                currentIndex++;
            }

            return currentIndex == record.DamagedGroupsCount.Count;
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var unfoldedRecords = records.Select(r =>
                {
                    var newString = string.Join('?', Enumerable.Repeat(r.DamagedRecord, 5));
                    var newList = Enumerable.Repeat(r.DamagedGroupsCount, 5)
                        .SelectMany(x => x)
                        .ToList();

                    return new Record
                    {
                        DamagedGroupsCount = newList,
                        DamagedRecord = newString,
                    };
                })
                .ToList();

            var result = unfoldedRecords.Select((r, idx) => CalculateArrangements(r)).Sum();

            if (result <= 26363835031)
            {
                throw new Exception("Too low");
            }

            return result.ToString();
        }

        public override async Task ReadInput()
        {
            records = await new RecordReader().ReadInputFromFile();
        }

        private List<Record> records;

        class Record
        {
            public string DamagedRecord { get; set; }

            public List<int> DamagedGroupsCount { get; set; }
        }

        class RecordReader : FileReader<Record>
        {
            protected override Record ProcessLineOfFile(string line)
            {
                var splitLine = line.Split(' ');

                var groups = splitLine[1]
                    .Split(',')
                    .Select(int.Parse)
                    .ToList();

                return new Record
                {
                    DamagedRecord = splitLine[0],
                    DamagedGroupsCount = groups,
                };
            }
        }
    }
}
