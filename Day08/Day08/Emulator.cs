using System;
using System.Collections.Generic;

namespace Day08
{
    public class Emulator
    {
        #region Fields

        private readonly HashSet<int> lineNumbersExecuted;

        #endregion

        #region Constructors

        public Emulator()
        {
            lineNumbersExecuted = new HashSet<int>();
        }

        #endregion

        #region Enums

        public enum ResultCode
        {
            ExecutionComplete,

            Deadlock,

            Fault
        }

        #endregion

        #region Instance Properties

        public int AccumulatorValue { get; private set; }

        #endregion

        #region Instance Methods

        public ResultCode RunOperations(List<Operation> operations)
        {
            AccumulatorValue = 0;
            lineNumbersExecuted.Clear();

            var lineNumberToExecute = 0;

            ResultCode result;

            while (true)
            {
                if (lineNumberToExecute < 0)
                {
                    // Shouldn't happen
                    result = ResultCode.Fault;
                    break;
                }

                if (lineNumberToExecute == operations.Count + 1)
                {
                    // We've reached the end of the file
                    result = ResultCode.ExecutionComplete;
                    break;
                }

                if (lineNumbersExecuted.Contains(lineNumberToExecute))
                {
                    // We've hit a deadlock
                    result = ResultCode.Deadlock;
                    break;
                }

                lineNumbersExecuted.Add(lineNumberToExecute);

                var operationToExecute = operations[lineNumberToExecute];

                switch (operationToExecute.OperationType)
                {
                    case OperationType.NOP:
                        lineNumberToExecute++;
                        break;
                    case OperationType.JMP:
                        lineNumberToExecute += operationToExecute.Argument;
                        break;
                    case OperationType.ACC:
                        AccumulatorValue += operationToExecute.Argument;
                        lineNumberToExecute++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Invalid operation type {operationToExecute.OperationType} detected.");
                }
            }

            return result;
        }

        #endregion
    }
}