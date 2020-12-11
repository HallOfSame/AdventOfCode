using System;
using System.Collections.Generic;
using System.Linq;

using IntCodeInterpreter.Extensions;
using IntCodeInterpreter.Models;
using IntCodeInterpreter.Models.Instructions;
using IntCodeInterpreter.Models.Instructions.Arithmetic;
using IntCodeInterpreter.Models.Instructions.IO;

namespace IntCodeInterpreter
{
    public class Interpreter
    {
        #region Instance Methods

        public void ProcessOperations(List<int> memory)
        {
            ProcessOperations(memory,
                              0,
                              x =>
                              {
                              });
        }

        public void ProcessOperations(List<int> mem,
                                      int input,
                                      Action<int> onOutput)
        {
            var memory = mem.ToArray();

            var instructionPointer = 0;

            while (true)
            {
                var currentOp = memory[instructionPointer];

                if (!Enum.TryParse<OpCode>((currentOp % 100).ToString(),
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
                    case OpCode.Input:
                        instruction = BuildInputInstruction(memory.Skip(instructionPointer)
                                                                  .Take(2)
                                                                  .ToArray());
                        memory[((InputInstruction)instruction).Destination.Value] = input;
                        break;
                    case OpCode.Output:
                        instruction = BuildOutputInstruction(memory.Skip(instructionPointer)
                                                                   .Take(2)
                                                                   .ToArray());

                        onOutput(GetParameterValue(((OutputInstruction)instruction).Source,
                                                   memory));
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
            var opOne = new Parameter(instruction[1],
                                      GetMode(instruction[0],
                                              1));
            var opTwo = new Parameter(instruction[2],
                                      GetMode(instruction[0],
                                              2));
            var dest = new Parameter(instruction[3],
                                     GetMode(instruction[0],
                                             3));

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

        private InputInstruction BuildInputInstruction(int[] instruction)
        {
            return new InputInstruction(new Parameter(instruction[1],
                                                      GetMode(instruction[0],
                                                              1)));
        }

        private OutputInstruction BuildOutputInstruction(int[] instruction)
        {
            return new OutputInstruction(new Parameter(instruction[1],
                                                       GetMode(instruction[0],
                                                               1)));
        }

        private ParameterMode GetMode(int instruction,
                                      int parameterNumber)
        {
            var div = (int)Math.Pow(10,
                                    parameterNumber + 1);

            var param = instruction / div % 10;

            return (ParameterMode)param;
        }

        private int GetParameterValue(Parameter param,
                                      int[] memory)
        {
            switch (param.Mode)
            {
                case ParameterMode.Position:
                    return memory[param.Value];
                case ParameterMode.Immediate:
                    return param.Value;
            }

            throw new ArgumentOutOfRangeException(nameof(param.Mode));
        }

        private void ProcessArithmeticOperation(ArithmeticInstruction instruction,
                                                int[] memory)
        {
            var valOne = GetParameterValue(instruction.OperandOne,
                                           memory);
            var valTwo = GetParameterValue(instruction.OperandTwo,
                                           memory);
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