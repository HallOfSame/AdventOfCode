using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
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

        class Rule
        {
            public Comparison Comparison { get; set; }

            public int Value { get; set; }

            public char Target { get; set; }

            public string Destination { get; set; }
        }
    }
}
