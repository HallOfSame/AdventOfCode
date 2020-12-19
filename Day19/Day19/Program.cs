using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day19
{
    internal class Program
    {
        #region Class Methods

        private static void Main(string[] args)
        {
            var rules = new List<GrammarRule>();

            rules.Add(new GrammarRule('S',
                                      new List<string>
                                      {
                                          "IE"
                                      }));

            rules.Add(new GrammarRule('I',
                                      new List<string>
                                      {
                                          "DA"
                                      }));

            rules.Add(new GrammarRule('A',
                                      new List<string>
                                      {
                                          "BC",
                                          "CB"
                                      }));

            rules.Add(new GrammarRule('B',
                                      new List<string>
                                      {
                                          "DD",
                                          "EE"
                                      }));

            rules.Add(new GrammarRule('C',
                                      new List<string>
                                      {
                                          "DE",
                                          "ED"
                                      }));

            rules.Add(new GrammarRule('D',
                                      new List<string>
                                      {
                                          "a"
                                      }));

            rules.Add(new GrammarRule('E',
                                      new List<string>
                                      {
                                          "b"
                                      }));

            var solver = new CYKSolver(rules);

            void CheckString(string inputString)
            {
                var isMatch = solver.MatchesGrammar(inputString);

                Console.WriteLine($"{inputString} matches? {isMatch}.");
            }

            CheckString("ababbb");
            CheckString("abbbab");
            CheckString("bababa");
            CheckString("aaabbb");
            CheckString("aaaabbb");
        }

        #endregion
    }

    public class CYKSolver
    {
        private readonly List<GrammarRule> rules;

        public CYKSolver(List<GrammarRule> rules)
        {
            if (rules.Any(r => r.Right.Any(rhs => rhs.Length > 2)))
            {
                throw new ArgumentException($"Class does not support rules with 3 outcomes. Break in to sub rules.");
            }

            this.rules = rules;
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
            for(var i = 1; i <= inputLength; i++)
            {
                for(var j = 1; j <= inputLength; j++)
                {
                    for (var k = 1; k <= rules.Count; k++)
                    {
                        table[i, j, k] = false;
                    }
                }
            }

            // Now initialize the single characters
            for(var i = 1; i <= inputLength; i++)
            {
                var characterInInput = input[i - 1];

                foreach (var rule in rules)
                {
                    if (rule.Right.Any(x => x[0] == characterInInput))
                    {
                        // Starting at i, the single character can be derived by this rule
                        table[i, 1, rules.IndexOf(rule) + 1] = true;
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
                        foreach (var rule in rules)
                        {
                            foreach (var option in rule.Right)
                            {
                                // Not terminal if capitalized
                                var isNonTerminal = option.Length == 2;

                                if (isNonTerminal)
                                {
                                    // Find the rule indexes that match up with the rule we are checking
                                    // i.e. if our rule is B -> CD
                                    // We're finding the index of rule C and D
                                    var b = rules.FindIndex(0, r => r.Representation == option[0]) + 1;
                                    var c = rules.FindIndex(0, r => r.Representation == option[1]) + 1;

                                    Debug.Assert(b > 0 && c > 0);

                                    // If we can derive the input starting at j with k char from rule b
                                    // And we can derive the input starting at j+k with i-k char from rule c
                                    if (table[j, k, b] && table[j + k, i - k, c])
                                    {
                                        // Then we must be able to derive the input starting at j with i char from the rule we are checking
                                        table[j, i, rules.IndexOf(rule) + 1] = true;

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
            }


            // Can we derive a string
            // Starting a pos 1 (1 based index)
            // Of length (input length)
            // Using rule 1 (1 based index) aka the start rule
            return table[1, inputLength, 1];
        }
    }

    [DebuggerDisplay("{Representation.ToString()} {string.Join(\" | \", Right)}")]
    public class GrammarRule
    {
        public GrammarRule(char representation,
                           List<string> right)
        {
            Representation = representation;
            Right = right;
        }

        public char Representation { get; }

        public List<string> Right { get; }
    }
}