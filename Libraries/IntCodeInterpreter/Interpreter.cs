using System;
using System.Collections.Generic;
using System.Linq;

using IntCodeInterpreter.Extensions;
using IntCodeInterpreter.Models;
using IntCodeInterpreter.Models.Instructions;
using IntCodeInterpreter.Models.Instructions.Arithmetic;

namespace IntCodeInterpreter
{
    public class Interpreter
    {
        #region Instance Methods

        public void ProcessOperations(List<int> memory)
        {
            var instructionPointer = 0;

            while (true)
            {
                var currentOp = memory[instructionPointer];

                if (!Enum.TryParse<OpCode>(currentOp.ToString(),
                                           out var opCode))
                {
                    opCode = OpCode.Unknown;
                }

                if (opCode == OpCode.EndExecution)
                {
                    return;
                }

                Instruction instruction;

                switch (opCode)
                {
                    case { } arithmeticCode when opCode.IsArithmetic():
                        instruction = BuildArithmeticInstruction(memory.Skip(instructionPointer)
                                                                       .Take(4)
                                                                       .ToArray(),
                                                                 arithmeticCode);

                        ProcessArithmeticOperation((ArithmeticInstruction)instruction,
                                                   memory);
                        break;
                    case OpCode.Unknown:
                        throw new InvalidOperationException("Unknown op code encountered.");
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                instructionPointer += instruction.InstructionPointerIncrement;
            }
        }

        private ArithmeticInstruction BuildArithmeticInstruction(int[] instruction,
                                                                 OpCode opCode)
        {
            var opOne = new Parameter(instruction[1]);
            var opTwo = new Parameter(instruction[2]);
            var dest = new Parameter(instruction[3]);

            switch (opCode)
            {
                case OpCode.Add:
                    return new AddInstruction(opOne,
                                              opTwo,
                                              dest);
                case OpCode.Multiply:
                    return new MultiplyInstruction(opOne,
                                                   opTwo,
                                                   dest);
            }

            throw new ArgumentOutOfRangeException(nameof(opCode));
        }

        private void ProcessArithmeticOperation(ArithmeticInstruction instruction,
                                                List<int> memory)
        {
            var valOne = memory[instruction.OperandOne.Value];
            var valTwo = memory[instruction.OperandTwo.Value];
            int result;

            switch (instruction.OpCode)
            {
                case OpCode.Add:
                    result = valOne + valTwo;
                    break;
                case OpCode.Multiply:
                    result = valOne * valTwo;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(instruction.OpCode));
            }

            memory[instruction.Destination.Value] = result;
        }

        #endregion
    }
}