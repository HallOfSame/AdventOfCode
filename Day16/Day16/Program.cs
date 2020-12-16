using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day16
{
    internal class Program
    {
        #region Class Methods

        private static void Main(string[] args)
        {
            Ticket yourTicket = null;

            var otherTickets = new List<Ticket>();

            var fieldRules = new Dictionary<string, List<FieldRule>>();

            using (var file = File.OpenRead("PuzzleInput.txt"))
            {
                using var reader = new StreamReader(file);

                var processingRules = true;
                var processingYourTicket = false;
                var processingOtherTickets = false;

                while (!reader.EndOfStream)
                {
                    var nextLine = reader.ReadLine();

                    if (processingRules)
                    {
                        if (string.IsNullOrEmpty(nextLine))
                        {
                            processingRules = false;
                            processingYourTicket = true;
                            continue;
                        }

                        var ruleSplit = nextLine.Split(':');

                        var fieldName = ruleSplit[0]
                            .Trim();

                        var fieldInfo = ruleSplit[1]
                                        .Trim()
                                        .Split(" or ");

                        var rules = fieldInfo.Select(x =>
                                                     {
                                                         var split = x.Split('-');

                                                         var lower = int.Parse(split[0]);
                                                         var upper = int.Parse(split[1]);

                                                         return new FieldRule(lower,
                                                                              upper);
                                                     })
                                             .ToList();

                        fieldRules.Add(fieldName,
                                       rules);
                    }
                    else if (processingYourTicket)
                    {
                        if (string.IsNullOrEmpty(nextLine))
                        {
                            processingYourTicket = false;
                            processingOtherTickets = true;
                            continue;
                        }

                        if (!nextLine.Contains("your ticket"))
                        {
                            yourTicket = new Ticket(nextLine.Split(',')
                                                            .Select(int.Parse)
                                                            .ToList());
                        }
                    }
                    else if (processingOtherTickets)
                    {
                        if (string.IsNullOrEmpty(nextLine))
                        {
                            continue;
                        }

                        if (!nextLine.Contains("nearby"))
                        {
                            otherTickets.Add(new Ticket(nextLine.Split(',')
                                                                .Select(int.Parse)
                                                                .ToList()));
                        }
                    }
                }
            }

            var invalidFields = new List<int>();
            var validTickets = new List<Ticket>();

            foreach (var ticket in otherTickets)
            {
                var invalidFieldsForTicket = new List<int>();

                for (var i = 0; i < ticket.Values.Count; i++)
                {
                    var value = ticket.Values[i];

                    var validRules = new List<string>();

                    foreach (var rule in fieldRules)
                    {
                        if (rule.Value.Any(x => x.IsValid(value)))
                        {
                            validRules.Add(rule.Key);
                        }
                    }

                    if (validRules.Count == 0)
                    {
                        invalidFieldsForTicket.Add(value);
                    }
                }

                if (invalidFieldsForTicket.Any())
                {
                    invalidFields.AddRange(invalidFieldsForTicket);
                }
                else
                {
                    validTickets.Add(ticket);
                }
            }

            Console.WriteLine($"Ticket scan error rate is {invalidFields.Sum()}.");

            var validPositionMap = new Dictionary<string, List<int>>();

            var validFieldsFromTickets = validTickets.Select(x => x.Values);

            var totalPositions = yourTicket.Values.Count;

            // First iterate each rule
            foreach (var rule in fieldRules)
            {
                var validPositionsForRule = new List<int>();

                // Go through every position in all valid tickets
                // Build up a list of possible positions that could be this rule
                for (var i = 0; i < totalPositions; i++)
                {
                    var validValuesForThisPosition = validFieldsFromTickets.Select(x => x[i])
                                                                           .ToList();

                    if (validValuesForThisPosition.All(x => rule.Value.Any(r => r.IsValid(x))))
                    {
                        validPositionsForRule.Add(i);
                    }
                }

                validPositionMap[rule.Key] = validPositionsForRule;
            }

            var rulePositionMap = new Dictionary<string, int>();

            // This while works since the input always works out nice, i.e. we never have a case where two rules could be either field
            while (validPositionMap.Any())
            {
                // Find the rules where they only have one possible position
                var singleMatchingPosition = validPositionMap.Where(x => x.Value.Count == 1)
                                                             .ToList();

                singleMatchingPosition.ForEach(x =>
                                               {
                                                   var foundPosition = x.Value.First();

                                                   // Add that position to our map
                                                   rulePositionMap.Add(x.Key,
                                                                       foundPosition);

                                                   // Remove the rule from further consideration
                                                   validPositionMap.Remove(x.Key);

                                                   // Go through the rest of the rules and remove that position from consideration
                                                   foreach (var remainingValue in validPositionMap.Values)
                                                   {
                                                       remainingValue.Remove(foundPosition);
                                                   }
                                               });
            }

            var positionsToCheck = rulePositionMap.Where(x => x.Key.StartsWith("departure"))
                                                  .Select(x => x.Value)
                                                  .ToList();

            var partTwoAnswer = positionsToCheck.Select(x => yourTicket.Values[x])
                                                .Aggregate(1L,
                                                           (x,
                                                            y) => x * y);

            Console.WriteLine($"Part two answer: {partTwoAnswer}.");
        }

        #endregion
    }

    public class Ticket
    {
        #region Constructors

        public Ticket(List<int> values)
        {
            Values = values;
        }

        #endregion

        #region Instance Properties

        public List<int> Values { get; }

        #endregion
    }

    public class FieldRule
    {
        #region Constructors

        public FieldRule(int lower,
                         int upper)
        {
            Lower = lower;
            Upper = upper;
        }

        #endregion

        #region Instance Properties

        public int Lower { get; }

        public int Upper { get; }

        #endregion

        #region Instance Methods

        public bool IsValid(int testValue)
        {
            return testValue >= Lower && testValue <= Upper;
        }

        #endregion
    }
}