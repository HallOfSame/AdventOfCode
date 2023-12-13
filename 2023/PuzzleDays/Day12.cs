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
            var result = CalculatePossibleArrangements(records);

            if (result != 7843)
            {
                Console.WriteLine("Wrong Part 1 now :(");
            }

            return result.ToString();
        }

        private decimal CalculatePossibleArrangements(List<Record> records)
        {
            return records.Select(r => RecursiveCalculateArrangements(r.DamagedRecord,
                                                                      0,
                                                                      0,
                                                                      0,
                                                                      r.DamagedGroupsCount,
                                                                      new Dictionary<(int, int, int), decimal>()))
                .Sum();
        }

        private decimal RecursiveCalculateArrangements(string springString,
                                                       int currentIndex,
                                                       int currentGroupLength,
                                                       int currentGroupLimit,
                                                       List<int> remainingGroups,
                                                       Dictionary<(int, int, int), decimal> memo)
        {
            var memoKey = (currentIndex, currentGroupLength, remainingGroups.Count);

            if (memo.TryGetValue(memoKey,
                                 out var preCalcRes))
            {
                return preCalcRes;
            }

            if (currentIndex == springString.Length)
            {
                if (!remainingGroups.Any() && (currentGroupLength == 0 || (currentGroupLength == currentGroupLimit)))
                {
                    // String is valid
                    return 1;
                }

                // Ended string but didn't finish the final group
                return 0;
            }

            var currentChar = springString[currentIndex];

            // If this is a '.', did we stop a group? If so & it doesn't match, fail this string
            if (currentChar == '.')
            {
                if (currentGroupLength > 0 && currentGroupLength != currentGroupLimit)
                {
                    // This string isn't valid, we stopped a group that wasn't the full length
                    return 0;
                }

                currentGroupLength = 0;
            }

            // If this is a '#', are we continuing a group or starting a new one?
            if (currentChar == '#')
            {
                if (currentGroupLength == 0)
                {
                    if (!remainingGroups.Any())
                    {
                        // We have # but no groups to fill
                        return 0;
                    }

                    // Starting a new group
                    currentGroupLimit = remainingGroups.First();
                    remainingGroups = remainingGroups.Skip(1)
                        .ToList();
                    currentGroupLength = 1;
                }
                else
                {
                    currentGroupLength++;

                    if (currentGroupLength > currentGroupLimit)
                    {
                        // The current group has gone too long
                        return 0;
                    }
                }
            }

            // If this is a '?', see what values it can be
            if (currentChar == '?')
            {
                var questionArrangements = 0m;

                // Can be '.' if the current group is the right size or we aren't in a group at all
                if (currentGroupLength == 0 || currentGroupLength == currentGroupLimit)
                {
                    var withPeriod = RecursiveCalculateArrangements(springString,
                                                                    currentIndex + 1,
                                                                    0,
                                                                    currentGroupLimit,
                                                                    remainingGroups,
                                                                    memo);

                    questionArrangements += withPeriod;
                }

                // Can be '#' if we are not in a group or the current one needs more #
                if ((currentGroupLength == 0 && remainingGroups.Any()) || (currentGroupLength > 0 && currentGroupLength < currentGroupLimit))
                {
                    if (currentGroupLength == 0)
                    {
                        // Starting a new group with this hash
                        currentGroupLimit = remainingGroups.First();
                        remainingGroups = remainingGroups.Skip(1)
                            .ToList();
                    }

                    var withHash = RecursiveCalculateArrangements(springString,
                                                                  currentIndex + 1,
                                                                  currentGroupLength + 1,
                                                                  currentGroupLimit,
                                                                  remainingGroups,
                                                                  memo);

                    questionArrangements += withHash;
                }

                memo[memoKey] = questionArrangements;

                return questionArrangements;
            }

            var result = RecursiveCalculateArrangements(springString,
                                                        currentIndex + 1,
                                                        currentGroupLength,
                                                        currentGroupLimit,
                                                        remainingGroups,
                                                        memo);

            memo[memoKey] = result;

            return result;
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

            var result = CalculatePossibleArrangements(unfoldedRecords);

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
