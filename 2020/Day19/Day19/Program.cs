using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Day19
{
    internal class Program
    {
        #region Class Methods

        private static void Main(string[] args)
        {
            var fileLines = File.ReadAllLines("PuzzleInput.txt");

            var rules = new List<GrammarRule>();
            var testStrings = new List<string>();

            var parsingRules = true;

            // First build the initial rule list
            foreach(var line in fileLines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    // Rules stop at first blank line
                    parsingRules = false;
                    continue;
                }

                if (parsingRules)
                {
                    var splitLine = line.Split(':');

                    var ruleId = splitLine[0].Trim();

                    var ruleOptions = splitLine[1].Split('|');

                    var optionArray = ruleOptions.Select(x => x.Trim().Trim('"').Split(' ').ToArray()).ToList();

                    rules.Add(new GrammarRule(ruleId, optionArray));
                }
                else
                {
                    testStrings.Add(line);
                }
            }

            // Important that rule 0 is first
            rules = rules.OrderBy(x => x.Representation).ToList();

            // Can't have rules like 1: 2
            var ruleRepresentations = rules.Select(x => x.Representation).ToHashSet();

            var removableRules = rules.Where(r => r.Right.Count == 1 && r.Right.Any(op => op.Length == 1 && ruleRepresentations.Contains(op[0]))).ToList();

            if (removableRules.Any())
            {
                Console.WriteLine($"Need to remove rules {string.Join(",", removableRules)}");
                throw new NotImplementedException();
            }

            // Can't have rules like 1: 2 | 3
            var replaceableRules = rules.Where(r => r.Right.Any(op => op.Length == 1 && ruleRepresentations.Contains(op[0]))).ToList();

            if (replaceableRules.Any())
            {
                Console.WriteLine($"Need to replace rules {string.Join(",", replaceableRules)}");
                throw new NotImplementedException();
            }

            var rulesTooLong = rules.Where(r => r.Right.Any(op => op.Length > 2)).ToList();

            // Need only 2 length non terminals for the algorithm to work
            if (rulesTooLong.Any())
            {
                Console.WriteLine($"Rules with too many options {string.Join(",", rulesTooLong)}");
                throw new NotImplementedException("Not yet designed to handle fixing inputs like this.");
            }

            Console.WriteLine("RULES: ");
            rules.ForEach(Console.WriteLine);
            Console.WriteLine("");

            var solver = new CYKSolver(rules);

            var workingStrings = 0;
            var testCount = 0;

            foreach (var test in testStrings)
            {
                Console.WriteLine($"Testing {test}. {testCount++}");

                if (solver.MatchesGrammar(test))
                {
                    Console.WriteLine($"{test} is a match.");
                    workingStrings++;
                }
                else
                {
                    Console.WriteLine($"{test} is not a match.");
                }
            }

            Console.WriteLine($"Number of strings that match: {workingStrings}.");
        }

        #endregion
    }

    public class CYKSolver
    {
        private readonly List<GrammarRule> rules;

        private readonly List<GrammarRule> terminalRules;

        private readonly List<GrammarRule> nonTerminalRules;

        private readonly Dictionary<string, int> ruleIndexLookup;

        public CYKSolver(List<GrammarRule> rules)
        {
            if (rules.Any(r => r.Right.Any(rhs => rhs.Length > 2)))
            {
                throw new ArgumentException($"Class does not support rules with 3 outcomes. Break in to sub rules.");
            }

            this.rules = rules;

            this.terminalRules = rules.Where(r => r.Right.Count == 1 && r.Right[0].Length == 1).ToList();
            this.nonTerminalRules = rules.Where(r => r.Right.Count > 1 || r.Right[0].Length > 1).ToList();

            ruleIndexLookup = new Dictionary<string, int>(rules.Count);

            for(var i = 0; i < rules.Count; i++)
            {
                ruleIndexLookup.Add(rules[i].Representation, i + 1);
            }
        }

        public bool MatchesGrammar(string input)
        {
            var inputLength = input.Length;

            // Create the table and initialize all fields

            // How the table works.
            // NOTE: Pretty much everything is using a 1 based index instead of 0.
            // Thats how the pseudocode on wikipedia did it and I'm too afraid to break it trying to change that.
            // The value at table[x,y,z] represents the following:
            // If true:
            // A substring of input starting at positon x (where x = 1 is the first char)
            // Of length y
            // Can be derived using rule z
            var table = new bool[inputLength + 1, inputLength + 1, rules.Count + 1];

            // Initialize everything to false
            // Don't need, default is false
            //for(var i = 1; i <= inputLength; i++)
            //{
            //    for(var j = 1; j <= inputLength; j++)
            //    {
            //        for (var k = 1; k <= rules.Count; k++)
            //        {
            //            table[i, j, k] = false;
            //        }
            //    }
            //}
            
            // Now initialize the single characters
            for(var i = 1; i <= inputLength; i++)
            {
                var characterInInput = input[i - 1];

                foreach (var rule in terminalRules)
                {
                    if (rule.Right.Any(x => ((string)x[0])[0] == characterInInput))
                    {
                        // Starting at i, the single character can be derived by this rule
                        table[i, 1, ruleIndexLookup[rule.Representation]] = true;
                    }
                }
            }

            // Now do the work of checking spans of input where length is > 1
            // i Starts at 2. We don't want to check length 1 again
            // For comments assume inputLength is 6 and i is 3
            // i is the length of total input we are checking
            for(var i = 2; i <= inputLength; i++)
            {
                // j goes from 1 to (6 - 3 + 1) 4
                // j drives the first span of input we check
                for(var j = 1; j <= inputLength - i + 1; j++)
                {
                    // k goes from 1 to (3 - 1) 2
                    // k drives the second span of input we check
                    for (var k = 1; k <= i - 1; k++)
                    {
                        foreach (var rule in nonTerminalRules)
                        {
                            foreach (var option in rule.Right)
                            {
                                // Find the rule indexes that match up with the rule we are checking
                                // i.e. if our rule is B -> CD
                                // We're finding the index of rule C and D
                                var b = ruleIndexLookup[option[0]];
                                var c = ruleIndexLookup[option[1]];

                                // If we can derive the input starting at j with k char from rule b
                                // And we can derive the input starting at j+k with i-k char from rule c
                                if (table[j, k, b]
                                    && table[j + k, i - k, c])
                                {
                                    // Then we must be able to derive the input starting at j with i char from the rule we are checking
                                    table[j, i, ruleIndexLookup[rule.Representation]] = true;

                                    // Another way of putting it using: i = 3, j = 1, k = 2
                                    // We can derive the substring from 1 with 2 char with the first half of this rule
                                    // We can derive the substring from 3 (i.e. where the first substring ends) with 1 char with the second half of this rule
                                    // So we can derive the full 3 char string (i = 3, so we are looking at length 3 strings) with this rule
                                }

                            }
                        }
                    }
                }
            }


            // Can we derive a string
            // Starting a pos 1 (1 based index)
            // Of length (input length)
            // Using rule 1 (1 based index) aka the start rule
            return table[1, inputLength, 1];
        }
    }

    public class GrammarRule
    {
        public GrammarRule(string representation,
                           List<string[]> right)
        {
            Representation = representation;
            Right = right;
        }

        public string Representation { get; }

        public List<string[]> Right { get; }

        public override string ToString()
        {
            return $"{Representation}: {string.Join(" | ", Right.Select(x => string.Join(" ", x)))}";
        }
    }
}