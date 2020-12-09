using System;
using System.Collections.Generic;

using IntCodeInterpreter.Extensions;
using IntCodeInterpreter.Models;

namespace IntCodeInterpreter
{
    public class IntCodeInterpreter
    {
        #region Instance Methods

        public List<int> ProcessOperations(List<int> operations)
        {
            var operationIndex = 0;

            while (true)
            {
                var currentOp = operations[operationIndex];
                operationIndex += 4;

                if (!Enum.TryParse<OpCode>(currentOp.ToString(),
                                           out var opCode))
                {
                    opCode = OpCode.Unknown;
                }

                if (opCode == OpCode.EndExecution)
                {
                    return operations;
                }

                if (opCode.IsArithmetic())
                {
                    ProcessArithmeticOperation(opCode,
                                               operations[operationIndex - 3],
                                               operations[operationIndex - 2],
                                               operations[operationIndex - 1],
                                               operations);
                    continue;
                }

                switch (opCode)
                {
                    case OpCode.Unknown:
                        throw new InvalidOperationException("Unknown op code encountered.");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void ProcessArithmeticOperation(OpCode currentOp,
                                                int posOne,
                                                int posTwo,
                                                int outputPos,
                                                List<int> operations)
        {
            var valOne = operations[posOne];
            var valTwo = operations[posTwo];
            int result;

            switch (currentOp)
            {
                case OpCode.Add:
                    result = valOne + valTwo;
                    break;
                case OpCode.Multiply:
                    result = valOne * valTwo;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentOp));
            }

            operations[outputPos] = result;
        }

        #endregion
    }
}