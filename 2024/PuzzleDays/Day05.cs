using Helpers.Structure;

using System.Text.RegularExpressions;

namespace PuzzleDays
{
    public class Day05 : SingleExecutionPuzzle<Day05.ExecState>
    {
        public record ExecState(List<int[]> Updates, List<OrderingRule> Rules);

        public record OrderingRule(int BeforePage, int AfterPage);

        public override PuzzleInfo Info => new(2024, 05, "Print Queue");

        protected override async Task<ExecState> LoadInputState(string puzzleInput)
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

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var result = 0;

            foreach(var update in InitialState.Updates)
            {
                var valid = true;

                foreach(var rule in InitialState.Rules)
                {
                    var beforeIndex = Array.IndexOf(update, rule.BeforePage);
                    if (beforeIndex == -1)
                    {
                        // Not valid for this update
                        continue;
                    }

                    var afterIndex = Array.IndexOf(update, rule.AfterPage);
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
            }

            return result.ToString();
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            throw new NotImplementedException();
        }
    }
}
