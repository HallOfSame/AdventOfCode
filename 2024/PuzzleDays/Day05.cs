using Helpers.Structure;

using System.Text.RegularExpressions;
using InputStorageDatabase;

namespace PuzzleDays
{
    public class Day05 : SingleExecutionPuzzle<Day05.ExecState>
    {
        public record ExecState(List<int[]> Updates, List<OrderingRule> Rules);

        public record OrderingRule(int BeforePage, int AfterPage);

        public override PuzzleInfo Info => new(2024, 05, "Print Queue");

        protected override async Task<ExecState> LoadInputState(string puzzleInput, PuzzleInputType inputType)
        {
            var lines = puzzleInput.Split('\n');
            var readingRules = true;
            var rules = new List<OrderingRule>();
            var updates = new List<int[]>();
            var ruleRegex = new Regex(@"(\d+)\|(\d+)");

            foreach(var line in lines) 
            {
                if (readingRules)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        readingRules = false;
                        continue;
                    }

                    var newRule = ruleRegex.Match(line);

                    rules.Add(new OrderingRule(int.Parse(newRule.Groups[1].Value), int.Parse(newRule.Groups[2].Value)));
                    continue;
                }              
                
                if (string.IsNullOrEmpty (line)) 
                {
                    continue;
                }

                var updateSplit = line.Split(",");
                updates.Add(updateSplit.Select(int.Parse).ToArray());
            }

            return new ExecState(updates, rules);
        }

        private List<int> invalidUpdateIndices = [];

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            invalidUpdateIndices.Clear();
            var result = 0;

            for (var index = 0; index < this.InitialState.Updates.Count; index++)
            {
                var update = this.InitialState.Updates[index];
                var valid = true;

                foreach (var rule in this.InitialState.Rules)
                {
                    var beforeIndex = Array.IndexOf(update,
                                                    rule.BeforePage);
                    if (beforeIndex == -1)
                    {
                        // Not valid for this update
                        continue;
                    }

                    var afterIndex = Array.IndexOf(update,
                                                   rule.AfterPage);
                    if (afterIndex == -1)
                    {
                        // Not valid for this update
                        continue;
                    }

                    if (beforeIndex > afterIndex)
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    result += update[update.Length / 2];
                }
                else
                {
                    // For part 2
                    invalidUpdateIndices.Add(index);
                }
            }

            return result.ToString();
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            if (invalidUpdateIndices.Count == 0)
            {
                throw new InvalidOperationException("Run part one first");
            }

            var result = 0;

            foreach(var brokenUpdateIndex in invalidUpdateIndices)
            {
                var updateToFix = InitialState.Updates[brokenUpdateIndex];

                var orderedPages = GetOrderOfPages(updateToFix.ToHashSet());

                result += orderedPages[orderedPages.Count / 2];
            }

            return result.ToString();
        }

        private List<int> GetOrderOfPages(HashSet<int> validPages)
        {
            var allPageNumbers = this.InitialState.Rules.SelectMany(x => new [] {x.BeforePage, x.AfterPage }).Where(validPages.Contains).Distinct().ToList();
            var pagesRequiredBefore = new Dictionary<int, HashSet<int>>();

            foreach(var rule in this.InitialState.Rules)
            {
                if (!validPages.Contains(rule.AfterPage) || !validPages.Contains(rule.BeforePage))
                {
                    continue;
                }

                if (pagesRequiredBefore.TryGetValue(rule.AfterPage, out var existingSet))
                {
                    existingSet.Add(rule.BeforePage);
                }
                else
                {
                    pagesRequiredBefore[rule.AfterPage] = [rule.BeforePage];
                }
            }

            // Create a sorted order of all elements
            var orderedPages = new List<int>();
            // These are the pages that can come before any others
            var startNodes = allPageNumbers.Where(x => !pagesRequiredBefore.ContainsKey(x)).ToList();

            while(startNodes.Count > 0) 
            {
                var next = startNodes.First();
                startNodes.RemoveAt(0);

                orderedPages.Add(next);

                var pagesThatMustGoAfterNext = pagesRequiredBefore.Where(x => x.Value.Contains(next)).ToList();

                foreach(var neighborNode in pagesThatMustGoAfterNext)
                {
                    neighborNode.Value.Remove(next);
                    if (neighborNode.Value.Count == 0)
                    {
                        startNodes.Add(neighborNode.Key);
                    }
                }
            }

            return orderedPages;
        }
    }
}
