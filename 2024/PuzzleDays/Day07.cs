using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day07 : SingleExecutionPuzzle<Day07.ExecState>
    {
        public record ExecState(List<Operation> Operations);

        public record Operation(decimal Result, Stack<decimal> Values);

        public override PuzzleInfo Info => new(2024, 7, "Bridge Repair");

        protected override async Task<ExecState> LoadInputState(string puzzleInput)
        {
            var input = puzzleInput.Trim();
            var operations = new List<Operation>();
            
            foreach (var line in input.Split('\n'))
            {
                var lineSplit = line.Split(": ");
                var result = decimal.Parse(lineSplit[0]);
                // Reverse here because otherwise we get the values in a backwards order
                var values = new Stack<decimal>(lineSplit[1]
                                                .Split(' ')
                                                .Select(decimal.Parse)
                                                .Reverse());

                operations.Add(new Operation(result, values));
            }

            return new ExecState(operations);
        }

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var result = 0m;
            // We end up modifying the stack
            var currentState = new MessagePackStateCopier().Copy(InitialState);

            foreach (var op in currentState.Operations)
            {
                var first = op.Values.Pop();
                if (CanBeMadeValid(op.Result, first, op.Values))
                {
                    result += op.Result;
                }
            }

            return result.ToString(CultureInfo.InvariantCulture);
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            throw new NotImplementedException();
        }

        private bool CanBeMadeValid(decimal expectedValue, decimal currentValue, Stack<decimal> remainingValues)
        {
            if (remainingValues.Count == 0)
            {
                return currentValue == expectedValue;
            }

            // Try adding + or *
            var nextValue = remainingValues.Pop();
            var newCurrentValue = currentValue + nextValue;
            if (CanBeMadeValid(expectedValue, newCurrentValue, remainingValues))
            {
                return true;
            }

            newCurrentValue = currentValue * nextValue;
            if (CanBeMadeValid(expectedValue, newCurrentValue, remainingValues))
            {
                return true;
            }

            remainingValues.Push(nextValue);
            return false;
        }
    }
}
