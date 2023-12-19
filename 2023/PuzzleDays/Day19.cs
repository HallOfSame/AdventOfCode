using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Helpers.FileReaders;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day19 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var acceptedParts = parts.Where(IsAccepted);

            var result = acceptedParts.Select(x => x.X + x.M + x.S + x.A)
                                      .Sum();

            return result.ToString();
        }

        private bool IsAccepted(Part part)
        {
            // Everything starts at in
            var workflow = workflows["in"];

            while (true)
            {
                var destination = workflow.GetDestination(part);

                switch (destination)
                {
                    case "A":
                        return true;
                    case "R":
                        return false;
                    default:
                        workflow = workflows[destination];
                        break;
                }
            }
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var startPoint = workflows["in"];

            var acceptanceRules = FindAcceptedPaths(startPoint,
                                        new List<Rule>());

            // Luckily it seems like there is no overlap between ranges
            var validRanges = acceptanceRules.Select(CalculateCombinations);

            return validRanges.Sum().ToString();
        }

        private decimal CalculateCombinations(List<Rule> rules)
        {
            // Iterate the rules, clamping the value ranges as needed
            var xRange = new Range
                         {
                             Min = 1,
                             Max = 4000,
                         };
            var mRange = new Range
                         {
                             Min = 1,
                             Max = 4000,
                         };
            var aRange = new Range
                         {
                             Min = 1,
                             Max = 4000,
                         };
            var sRange = new Range
                         {
                             Min = 1,
                             Max = 4000,
                         };

            foreach (var rule in rules)
            {
                var targetRange = rule.Target switch
                {
                    'x' => xRange,
                    'm' => mRange,
                    'a' => aRange,
                    's' => sRange,
                    _ => throw new Exception()
                };

                if (rule.Comparison == Comparison.LT)
                {
                    if (targetRange.Max > rule.Value)
                    {
                        targetRange.Max = rule.Value - 1;
                    }
                }
                else
                {
                    if (targetRange.Min < rule.Value)
                    {
                        targetRange.Min = rule.Value + 1;
                    }
                }
            }

            // Check for invalid rulesets (not sure this applies)
            if (xRange.Max < xRange.Min || mRange.Max < mRange.Min || aRange.Max < aRange.Min || sRange.Max < sRange.Min)
            {
                return 0m;
            }

            var xValues = xRange.Max - xRange.Min + 1;
            var mValues = mRange.Max - mRange.Min + 1;
            var aValues = aRange.Max - aRange.Min + 1;
            var sValues = sRange.Max - sRange.Min + 1;

            return (decimal)xValues * mValues * aValues * sValues;
        }

        private List<List<Rule>> FindAcceptedPaths(Workflow workflow,
                                                   List<Rule> currentRules)
        {
            var results = new List<List<Rule>>();

            var invertedRules = new List<Rule>();

            // Go through each rule of the workflow
            foreach (var rule in workflow.Rules)
            {
                // The current ruleset is:
                // Any rules we have past already (inverted)
                // Plus the current rule
                var withThisRule = invertedRules.Concat(new[]
                                                        {
                                                            rule
                                                        });

                // If destination is A we have a solution
                if (rule.Destination == "A")
                {
                    results.Add(currentRules.Concat(withThisRule.ToList())
                                            .ToList());
                }
                // Otherwise if it isn't rejected, recurse
                else if (rule.Destination != "R")
                {
                    // Find paths after this rule
                    var optionsAfterThisRule = FindAcceptedPaths(workflows[rule.Destination],
                                                                 currentRules.Concat(withThisRule.ToList())
                                                                             .ToList());

                    optionsAfterThisRule.ForEach(results.Add);
                }

                // Further processing means we didn't match this rule, so invert it
                invertedRules.Add(rule.InvertedRule());
            }

            // If this default to accepted, then include all the inverted rules as a result
            if (workflow.DefaultDestination == "A")
            {
                results.Add(currentRules.Concat(invertedRules)
                                        .ToList());
            }
            else if (workflow.DefaultDestination != "R")
            {
                // Else, recurse and see what rules we hit in the next workflow
                var optionsAfterThisRule = FindAcceptedPaths(workflows[workflow.DefaultDestination],
                                                             currentRules.Concat(invertedRules)
                                                                         .ToList());

                optionsAfterThisRule.ForEach(results.Add);
            }

            return results;
        }

        private Dictionary<string, Workflow> workflows = new();

        private List<Part> parts = new();

        public override async Task ReadInput()
        {
            var strings = await new StringFileReader().ReadInputFromFile();

            var addingRules = true;

            var partRegex = new Regex(@"{x=(\d+),m=(\d+),a=(\d+),s=(\d+)}");

            foreach(var line in strings)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    addingRules = false;
                    continue;
                }

                if (addingRules)
                {
                    var split = line.Split('{');
                    var workflowName = split[0];

                    var restOfFlow = split[1][..^1];

                    var rulesSplit = restOfFlow.Split(',');

                    var ruleList = new List<Rule>();

                    foreach(var ruleData in rulesSplit[..^1])
                    {
                        var ruleDataSplit = ruleData.Split(':');

                        var destination = ruleDataSplit[1];

                        Comparison c;

                        if (ruleDataSplit[0]
                            .Contains("<"))
                        {
                            c = Comparison.LT;
                        }
                        else
                        {
                            c = Comparison.GT;
                        }

                        var ruleInfoSplit = ruleDataSplit[0]
                            .Split(c == Comparison.LT
                                       ? '<'
                                       : '>');

                        var target = ruleInfoSplit[0];
                        var value = int.Parse(ruleInfoSplit[1]);

                        ruleList.Add(new Rule
                                     {
                                         Destination = destination,
                                         Target = target[0],
                                         Value = value,
                                         Comparison = c,
                                     });
                    }

                    var defaultDest = rulesSplit[^1];

                    workflows[workflowName] = new Workflow
                                              {
                                                  DefaultDestination = defaultDest,
                                                  Rules = ruleList,
                                                  Name = workflowName
                                              };
                }
                else
                {
                    var match = partRegex.Match(line);

                    var part = new Part()
                    {
                        X = int.Parse(match.Groups[1].Value),
                        M = int.Parse(match.Groups[2].Value),
                        A = int.Parse(match.Groups[3].Value),
                        S = int.Parse(match.Groups[4].Value),
                    };

                    parts.Add(part);
                }
            }
        }

        class Range
        {
            public int Min { get; set; }

            public int Max { get; set; }

            public override string ToString()
            {
                return $"[{Min},{Max}]";
            }
        }

        class Part
        {
            public int X { get; set; }

            public int M { get; set; }

            public int A { get; set; }

            public int S { get; set; }
        }

        class Workflow
        {
            public string Name { get; set; }

            public List<Rule> Rules { get; set; } = new();

            public string DefaultDestination { get; set; }

            public string GetDestination(Part part)
            {
                foreach (var rule in Rules)
                {
                    var targetValue = rule.Target switch
                    {
                        'x' => part.X,
                        'm' => part.M,
                        'a' => part.A,
                        's' => part.S,
                        _ => throw new Exception()
                    };

                    if (rule.Comparison == Comparison.LT)
                    {
                        if (targetValue < rule.Value)
                        {
                            return rule.Destination;
                        }
                    }
                    else
                    {
                        if (targetValue > rule.Value)
                        {
                            return rule.Destination;
                        }
                    }
                }

                return DefaultDestination;
            }
        }

        enum Comparison
        {
            GT,
            LT
        }

        [DebuggerDisplay("{Target}{CompString}{Value}")]
        class Rule
        {
            public Comparison Comparison { get; set; }

            public string CompString
            {
                get
                {
                    return Comparison == Comparison.LT
                               ? "<"
                               : ">";
                }
            }

            public int Value { get; set; }

            public char Target { get; set; }

            public string Destination { get; set; }

            public Rule InvertedRule()
            {
                return new Rule
                       {
                           Target = Target,
                           Comparison = Comparison == Comparison.LT
                                            ? Comparison.GT
                                            : Comparison.LT,
                           Value = Comparison == Comparison.LT
                                       ? Value - 1
                                       : Value + 1
                       };
            }
        }
    }
}
