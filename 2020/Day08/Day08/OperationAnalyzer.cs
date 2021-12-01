using System;
using System.Collections.Generic;
using System.Linq;

namespace Day08
{
    public class OperationAnalyzer
    {
        #region Fields

        private readonly List<Operation> originalInstructions;

        #endregion

        #region Constructors

        public OperationAnalyzer(List<Operation> originalInstructions)
        {
            this.originalInstructions = originalInstructions;
        }

        #endregion

        #region Instance Methods

        public List<Operation> AnalyzeAndFixInstructions()
        {
            var unvisited = Enumerable.Range(0,
                                             originalInstructions.Count)
                                      .ToList();

            var valid = new HashSet<int>(originalInstructions.Count);
            var invalid = new HashSet<int>(originalInstructions.Count);

            // Loop through all the instructions
            while (unvisited.Any())
            {
                // Start at the next unvisited item
                var nextStart = unvisited.First();

                var lineNumberToExecute = nextStart;

                var instructionsExecuted = new HashSet<int>();

                while (true)
                {
                    if (lineNumberToExecute < 0)
                    {
                        // Shouldn't happen
                        throw new InvalidOperationException("Negative line to execute.");
                    }

                    // The program would halt, add all these to the valid list
                    if (lineNumberToExecute == originalInstructions.Count)
                    {
                        valid.UnionWith(instructionsExecuted);
                        break;
                    }

                    // The program would loop, add all these to the invalid list
                    if (instructionsExecuted.Contains(lineNumberToExecute))
                    {
                        invalid.UnionWith(instructionsExecuted);
                        break;
                    }

                    instructionsExecuted.Add(lineNumberToExecute);

                    var operationToExecute = originalInstructions[lineNumberToExecute];

                    switch (operationToExecute.OperationType)
                    {
                        case OperationType.NOP:
                            lineNumberToExecute++;
                            break;
                        case OperationType.JMP:
                            lineNumberToExecute += operationToExecute.Argument;
                            break;
                        case OperationType.ACC:
                            lineNumberToExecute++;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"Invalid operation type {operationToExecute.OperationType} detected.");
                    }
                }

                // Remove all unvisited instructions from the list and clean up
                unvisited.RemoveAll(x => instructionsExecuted.Contains(x));
                instructionsExecuted.Clear();
            }

            // Now start walking through the operations and find one to change
            (Operation op, int index)? corruptOperation = null;

            var nextOp = 0;

            while(true)
            {
                var op = originalInstructions[nextOp];

                switch (op.OperationType)
                {
                    case OperationType.JMP:
                        var indexIfNoOp = nextOp + 1;
                        if (valid.Contains(indexIfNoOp))
                        {
                            corruptOperation = (op, nextOp);
                        }

                        nextOp += op.Argument;

                        break;
                    case OperationType.NOP:
                        var indexIfJmp = nextOp + op.Argument;
                        if (valid.Contains(indexIfJmp))
                        {
                            corruptOperation = (op, nextOp);
                        }

                        nextOp++;
                        break;
                    case OperationType.ACC:
                        nextOp++;
                        break;
                }

                if (corruptOperation != null)
                {
                    break;
                }
            }

            if (corruptOperation == null)
            {
                throw new InvalidOperationException("No valid replacement op found.");
            }

            // Flip the operation type
            var fixedOp = new Operation(corruptOperation.Value.op.OperationType == OperationType.JMP
                                            ? OperationType.NOP
                                            : OperationType.JMP,
                                        corruptOperation.Value.op.Argument);

            var fixedInstructions = new List<Operation>(originalInstructions)
                                    {
                                        [corruptOperation.Value.index] = fixedOp
                                    };

            // Instead of updating the instructions and running the emulator again we could probably merge all this to cut down on time
            // Track the accumulator as we process, and run until end in the above loop

            return fixedInstructions;
        }

        #endregion
    }
}