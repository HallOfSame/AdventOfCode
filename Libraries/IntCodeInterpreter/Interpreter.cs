using IntCodeInterpreter.Extensions;
using IntCodeInterpreter.Models;
using IntCodeInterpreter.Models.Instructions;
using IntCodeInterpreter.Models.Instructions.Arithmetic;
using IntCodeInterpreter.Models.Instructions.Comparison;
using IntCodeInterpreter.Models.Instructions.FlowControl.Jump;
using IntCodeInterpreter.Models.Instructions.IO;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void ProcessOperations(List<int> mem, int input, Action<int> onOutput)
        {
            var calledOnce = false;

            ProcessOperations(mem, () =>
            {
                if (!calledOnce)
                {
                    return input;
                }

                throw new InvalidOperationException("Called for multiple inputs.");
            }, onOutput);
        }

        public void ProcessOperations(List<int> mem,
                                      Func<int> getInput,
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
                    mem = memory.ToList();

                    return;
                }

                Instruction instruction;

                int[] GetArrayForInstruction(int size)
                {
                    return memory.Skip(instructionPointer)
                                 .Take(size)
                                 .ToArray();
                }

                switch (opCode)
                {
                    case { } arithmeticCode when opCode.IsArithmetic():
                        instruction = BuildArithmeticInstruction(GetArrayForInstruction(4),
                                                                 arithmeticCode);

                        ProcessArithmeticOperation((ArithmeticInstruction)instruction,
                                                   memory);
                        break;
                    case { } jumpCode when opCode.IsJump():
                        instruction = BuildJumpInstruction(GetArrayForInstruction(3),
                                                           jumpCode);

                        var jumpInstruction = (JumpInstruction)instruction;

                        ProcessJumpOperation(jumpInstruction,
                                             ref instructionPointer,
                                             memory);
                        break;
                    case { } compareCode when opCode.IsComparison():
                        instruction = BuildCompareInstruction(GetArrayForInstruction(4),
                                                              compareCode);

                        var compareInstruction = (ComparisonInstruction)instruction;

                        ProcessCompareOperation(compareInstruction,
                                                memory);
                        break;
                    case OpCode.Input:
                        instruction = BuildInputInstruction(GetArrayForInstruction(2));

                        memory[((InputInstruction)instruction).Destination.Value] = getInput();
                        break;
                    case OpCode.Output:
                        instruction = BuildOutputInstruction(GetArrayForInstruction(2));

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

        private ComparisonInstruction BuildCompareInstruction(int[] instruction,
                                                              OpCode opCode)
        {
            var opOne = new Parameter(instruction[1],
                                      GetMode(instruction[0],
                                              1));
            var opTwo = new Parameter(instruction[2],
                                      GetMode(instruction[0],
                                              2));

            var destination = new Parameter(instruction[3],
                                            GetMode(instruction[0],
                                                    3));

            switch (opCode)
            {
                case OpCode.LessThan:
                    return new LessThanInstruction(opOne,
                                                   opTwo,
                                                   destination);
                case OpCode.Equals:
                    return new EqualsInstruction(opOne,
                                                 opTwo,
                                                 destination);
            }

            throw new ArgumentOutOfRangeException(nameof(opCode));
        }

        private InputInstruction BuildInputInstruction(int[] instruction)
        {
            return new InputInstruction(new Parameter(instruction[1],
                                                      GetMode(instruction[0],
                                                              1)));
        }

        private JumpInstruction BuildJumpInstruction(int[] instruction,
                                                     OpCode opCode)
        {
            var opOne = new Parameter(instruction[1],
                                      GetMode(instruction[0],
                                              1));
            var opTwo = new Parameter(instruction[2],
                                      GetMode(instruction[0],
                                              2));

            switch (opCode)
            {
                case OpCode.JumpIfFalse:
                    return new JumpIfFalseInstruction(opOne,
                                                      opTwo);
                case OpCode.JumpIfTrue:
                    return new JumpIfTrueInstruction(opOne,
                                                     opTwo);
            }

            throw new ArgumentOutOfRangeException(nameof(opCode));
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

        private void ProcessCompareOperation(ComparisonInstruction instruction,
                                             int[] memory)
        {
            var valueOne = GetParameterValue(instruction.ValueOne,
                                             memory);

            var valueTwo = GetParameterValue(instruction.ValueTwo,
                                             memory);

            int valueToSet;

            switch (instruction.OpCode)
            {
                case OpCode.LessThan:
                    valueToSet = valueOne < valueTwo
                                     ? 1
                                     : 0;
                    break;
                case OpCode.Equals:
                    valueToSet = valueOne == valueTwo
                                     ? 1
                                     : 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(instruction.OpCode));
            }

            memory[instruction.Destination.Value] = valueToSet;
        }

        private void ProcessJumpOperation(JumpInstruction instruction,
                                          ref int instructionPointer,
                                          int[] memory)
        {
            var value = GetParameterValue(instruction.Value,
                                          memory);

            bool setInstructionPointer;

            switch (instruction.OpCode)
            {
                case OpCode.JumpIfTrue:
                    setInstructionPointer = value != 0;
                    break;
                case OpCode.JumpIfFalse:
                    setInstructionPointer = value == 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(instruction.OpCode));
            }

            if (setInstructionPointer)
            {
                instructionPointer = GetParameterValue(instruction.InstructionPointer,
                                                       memory);
            }
            else
            {
                instructionPointer += 3;
            }
        }

        #endregion
    }
}