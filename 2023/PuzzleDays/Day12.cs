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

        private int CalculateArrangements(Record record)
        {
            return CalculateRecursive(record.DamagedRecord, record);
        }

        private int CalculateRecursive(string partialString, Record record)
        {
            var nextQuestionMark = partialString.IndexOf('?');

            if (nextQuestionMark == -1)
            {
                // Base case
                return 1;
            }

            var usingDot = partialString.ReplaceAt(nextQuestionMark, '.');
            var usingHash = partialString.ReplaceAt(nextQuestionMark, '#');
            var total = 0;

            if (IsValid(usingDot, record))
            {
                total += CalculateRecursive(usingDot, record);
            }

            if (IsValid(usingHash, record))
            {
                total += CalculateRecursive(usingHash, record);
            }

            return total;
        }

        private bool IsValid(string proposedRestoration, Record record)
        {
            var currentIndex = 0;
            var currentGroupSizeLimit = record.DamagedGroupsCount[currentIndex];
            var currentGroupSize = 0;
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

            var result = unfoldedRecords.Select((r, idx) =>
                {
                    if (idx % 10 == 0)
                    {
                        Console.WriteLine(idx);
                    }
                    return CalculateArrangements(r);
                }).Sum();

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
