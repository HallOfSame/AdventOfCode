using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DayOne.Interfaces;

namespace DayOne.Impl
{
    public class ValueSumFinder : IValueSumFinder
    {
        #region Fields

        private readonly IMathematicOperationEvaluator mathematicOperationEvaluator;

        private readonly IOperationFactory operationFactory;

        #endregion

        #region Constructors

        public ValueSumFinder(IOperationFactory operationFactory,
                              IMathematicOperationEvaluator mathematicOperationEvaluator)
        {
            this.operationFactory = operationFactory ?? throw new ArgumentNullException(nameof(operationFactory));
            this.mathematicOperationEvaluator = mathematicOperationEvaluator ?? throw new ArgumentNullException(nameof(mathematicOperationEvaluator));
        }

        #endregion

        #region Instance Methods

        public async Task<List<int>> GetValuesForSumAsync(List<int> values,
                                                          int numberOfValues,
                                                          int requiredSum)
        {
            // Rather lazy way of doing things but what are you gonna do

            var summationTasks = new Task<List<int>>[0];

            if (numberOfValues == 2)
            {
                summationTasks = values.Select((val,
                                                index) =>
                                               {
                                                   var remainingValues = values.ToList();

                                                   remainingValues.RemoveAt(index);

                                                   return Task.Run(() => GetMatchingValue(new List<int>
                                                                                          {
                                                                                              val
                                                                                          },
                                                                                          remainingValues,
                                                                                          requiredSum));
                                               })
                                       .ToArray();
            }

            if (numberOfValues == 3)
            {
                var listOfTasks = new List<Task<List<int>>>();

                for (var i = 0; i < values.Count; i++)
                {
                    var innerValues = values.ToList();

                    var valueOne = innerValues[i];

                    innerValues.RemoveAt(i);

                    for (var k = 0; k < innerValues.Count; k++)
                    {
                        var innerValuesTwoElectricBoogaloo = innerValues.ToList();

                        var valueTwo = innerValuesTwoElectricBoogaloo[k];

                        innerValuesTwoElectricBoogaloo.RemoveAt(k);

                        // Removing this uses 6+ GB of memory. Not recommended.
                        if (valueOne + valueTwo >= requiredSum)
                        {
                            continue;
                        }

                        var setValues = new List<int>
                                        {
                                            valueOne,
                                            valueTwo
                                        };

                        listOfTasks.AddRange(innerValuesTwoElectricBoogaloo.Select((val,
                                                                                    index) =>
                                                                                   {
                                                                                       var remainingValues = values.ToList();

                                                                                       remainingValues.RemoveAt(index);

                                                                                       return Task.Run(() => GetMatchingValue(setValues,
                                                                                                                              remainingValues,
                                                                                                                              requiredSum));
                                                                                   }));
                    }
                }

                summationTasks = listOfTasks.ToArray();
            }

            await Task.WhenAll(summationTasks);

            var correctPair = summationTasks.Select(x => x.Result)
                                            .FirstOrDefault(x => x != null);

            if (correctPair == null)
            {
                throw new InvalidOperationException("No matching pairs found in input.");
            }

            return correctPair;
        }

        private List<int> GetMatchingValue(List<int> setValues,
                                           List<int> finalValueOptions,
                                           int requiredSum)
        {
            var operands = finalValueOptions.Select(x =>
                                                    {
                                                        var vals = setValues.ToList();
                                                        vals.Add(x);
                                                        return vals;
                                                    })
                                            .ToList();

            var operations = operands.Select(x => operationFactory.CreateAddOperation(x));

            foreach (var op in operations)
            {
                var result = mathematicOperationEvaluator.EvaluateOperation(op);

                if (result == requiredSum)
                {
                    return op.Operands;
                }
            }

            return null;
        }

        #endregion
    }
}