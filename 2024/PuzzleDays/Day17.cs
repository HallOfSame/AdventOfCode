using Helpers.Structure;

namespace PuzzleDays
{
    public class Day17 : SingleExecutionPuzzle<Day17.ExecState>
    {
        public record ExecState(int[] Program, int RegisterA, int RegisterB, int RegisterC);

        public override PuzzleInfo Info => new(2024, 17, "Chronospatial Computer");
        protected override async Task<ExecState> LoadInputState(string puzzleInput)
        {
            var lines = puzzleInput.Trim()
                .Split('\n');

            var a = int.Parse(lines[0]
                                  .Split(": ")[1]);
            var b = int.Parse(lines[1]
                                  .Split(": ")[1]);
            var c = int.Parse(lines[2]
                                  .Split(": ")[1]);

            var programStr = lines[4]
                .Split(": ")[1];

            var program = programStr.Split(',')
                .Select(int.Parse)
                .ToArray();

            return new ExecState(program, a, b, c);
        }

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var computer = new Computer(InitialState.Program,
                                        new Registers
                                        {
                                            A = InitialState.RegisterA,
                                            B = InitialState.RegisterB,
                                            C = InitialState.RegisterC
                                        });

            var result = computer.Execute();
            return result;
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            throw new NotImplementedException();
        }

        private class Computer(int[] program, Registers initialMemory)
        {
            private Registers Memory { get; } = initialMemory;
            private int[] Program { get; } = program;
            private List<int> Output { get; } = [];
            private int instructionPointer;

            public string Execute()
            {
                while (instructionPointer <= Program.Length - 2)
                {
                    var opCode = (OpCode)Program[instructionPointer++];
                    var operand = Program[instructionPointer++];

                    Operation operation = opCode switch
                    {
                        OpCode.Adv => new DivideAOperation(),
                        OpCode.Bxl => new BitwiseXorBOperation(),
                        OpCode.Bst => new BStoreOperation(),
                        OpCode.Jnz => new JumpNotZeroOperation(),
                        OpCode.Bxc => new BitwiseXorCOperation(),
                        OpCode.Out => new OutOperation(),
                        OpCode.Bdv => new BDivisionOperation(),
                        OpCode.Cdv => new CDivisionOperation(),
                        _ => throw new InvalidOperationException($"Invalid op code {opCode}")
                    };

                    operation.Execute(operand, Memory, Output, ref instructionPointer);
                }

                return string.Join(",", Output);
            }
        }

        public class Registers
        {
            public int A { get; set; }
            public int B { get; set; }
            public int C { get; set; }
        }

        public enum OpCode
        {
            Adv = 0,
            Bxl = 1,
            Bst = 2,
            Jnz = 3,
            Bxc = 4,
            Out = 5,
            Bdv = 6,
            Cdv = 7
        }

        public abstract class Operation
        {
            public void Execute(int operand, Registers memory, List<int> output, ref int instructionPointer)
            {
                var operandValue = GetOperandValue(operand, memory);
                ProcessInstruction(operandValue, memory, output, ref instructionPointer);
            }

            protected abstract void ProcessInstruction(int operandValue,
                                                       Registers memory,
                                                       List<int> output,
                                                       ref int instructionPointer);
            protected abstract int GetOperandValue(int operand, Registers memory);
        }

        public abstract class LiteralOperation : Operation
        {
            protected sealed override int GetOperandValue(int operand, Registers memory)
            {
                // Literal means the value is just the value
                return operand;
            }
        }

        public abstract class ComboOperation : Operation
        {
            protected sealed override int GetOperandValue(int operand, Registers memory)
            {
                return operand switch
                {
                    // 0-3 is literal value
                    >= 0 and <= 3 => operand,
                    4 => memory.A,
                    5 => memory.B,
                    6 => memory.C,
                    _ => throw new InvalidOperationException($"Invalid operand {operand}")
                };
            }
        }

        public abstract class DivideOperation : ComboOperation
        {
            protected sealed override void ProcessInstruction(int operandValue,
                                                              Registers memory,
                                                              List<int> output,
                                                              ref int instructionPointer)
            {
                var numerator = memory.A;
                var denominator = Math.Pow(2, operandValue);
                var fullPrecisionResult = numerator / denominator;
                var result = (int)Math.Truncate(fullPrecisionResult);
                StoreResult(result, memory);
            }

            protected abstract Action<int, Registers> StoreResult { get; }
        }

        public class DivideAOperation : DivideOperation
        {
            protected override Action<int, Registers> StoreResult { get; } = (val, mem) => mem.A = val;
        }

        public class BitwiseXorBOperation : LiteralOperation
        {
            protected override void ProcessInstruction(int operandValue,
                                                       Registers memory,
                                                       List<int> output,
                                                       ref int instructionPointer)
            {
                var result = memory.B ^ operandValue;
                memory.B = result;
            }
        }

        public class BStoreOperation : ComboOperation
        {
            protected override void ProcessInstruction(int operandValue,
                                                       Registers memory,
                                                       List<int> output,
                                                       ref int instructionPointer)
            {
                var result = operandValue % 8;
                memory.B = result;
            }
        }

        public class JumpNotZeroOperation : LiteralOperation
        {
            protected override void ProcessInstruction(int operandValue, Registers memory, List<int> output, ref int instructionPointer)
            {
                if (memory.A == 0)
                {
                    return;
                }

                instructionPointer = operandValue;
            }
        }

        public class BitwiseXorCOperation : LiteralOperation
        {
            protected override void ProcessInstruction(int operandValue, Registers memory, List<int> output, ref int instructionPointer)
            {
                var result = memory.B ^ memory.C;
                memory.B = result;
            }
        }

        public class OutOperation : ComboOperation
        {
            protected override void ProcessInstruction(int operandValue, Registers memory, List<int> output, ref int instructionPointer)
            {
                output.Add(operandValue % 8);
            }
        }

        public class BDivisionOperation : DivideOperation
        {
            protected override Action<int, Registers> StoreResult { get; } = (val, mem) => mem.B = val;
        }

        public class CDivisionOperation : DivideOperation
        {
            protected override Action<int, Registers> StoreResult { get; } = (val, mem) => mem.C = val;
        }
    }
}
