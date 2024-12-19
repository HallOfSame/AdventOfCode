using Helpers.Structure;
using InputStorageDatabase;

namespace PuzzleDays
{
    public class Day17 : SingleExecutionPuzzle<Day17.ExecState>
    {
        public record ExecState(int[] Program, int RegisterA, int RegisterB, int RegisterC);

        public override PuzzleInfo Info => new(2024, 17, "Chronospatial Computer");
        protected override async Task<ExecState> LoadInputState(string puzzleInput, PuzzleInputType inputType)
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
            var validAs = FindValidValuesForA([0], 1, string.Join(",", InitialState.Program));

            return validAs.Min()
                .ToString();
        }

        private List<long> FindValidValuesForA(List<long> validValues, int subLength, string expectedProgramString)
        {
            while (true)
            {
                if (subLength > expectedProgramString.Length)
                {
                    // We found the full string, so return
                    return validValues;
                }

                var newValids = new List<long>();
                checked
                {
                    // The gist of the program is:
                    // Get the last 3 bytes of A
                    // Flip the bits
                    // Discard that number of bytes from A
                    // XOR that number of bytes with the result of the discard
                    // Flip the bits again
                    // Output that % 8
                    // So the end of the output is controlled by the octal values at the start of the A register
                    // Or in other words, changing the end does not change the ending output values
                    // So what we do here is:
                    // Starting with a list of validValues (these when ran create the correct output ending substring
                    // Shift right 3 bits (aka * 8)
                    // Adding another 3 bits gets the next digit of output
                    // If it matches the increased substring add it to the list to check next iteration
                    // Cry several times because int overflow doesn't throw by default
                    // Eventually we get to where we built the full string, the search space is small so it runs quickly
                    // Then for the answer we just grab the min
                    foreach (var startVal in validValues.Select(val => val * 8))
                    {
                        for (var i = 0; i <= 7; i++)
                        {
                            var testValue = startVal + i;
                            var computer = new Computer(InitialState.Program,
                                                        new Registers { A = testValue, B = InitialState.RegisterB, C = InitialState.RegisterC });

                            var result = computer.Execute();

                            if (result.Equals(expectedProgramString[^subLength..]))
                            {
                                newValids.Add(testValue);
                            }
                        }
                    }
                }

                validValues = newValids;
                subLength += 2;
            }
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
            public long A { get; set; }
            public long B { get; set; }
            public long C { get; set; }
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

            protected abstract void ProcessInstruction(long operandValue,
                                                       Registers memory,
                                                       List<int> output,
                                                       ref int instructionPointer);
            protected abstract long GetOperandValue(int operand, Registers memory);
        }

        public abstract class LiteralOperation : Operation
        {
            protected sealed override long GetOperandValue(int operand, Registers memory)
            {
                // Literal means the value is just the value
                return operand;
            }
        }

        public abstract class ComboOperation : Operation
        {
            protected sealed override long GetOperandValue(int operand, Registers memory)
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
            protected sealed override void ProcessInstruction(long operandValue,
                                                              Registers memory,
                                                              List<int> output,
                                                              ref int instructionPointer)
            {
                var numerator = memory.A;
                var denominator = Math.Pow(2, operandValue);
                var fullPrecisionResult = numerator / denominator;
                var result = (long)Math.Truncate(fullPrecisionResult);
                StoreResult(result, memory);
            }

            protected abstract Action<long, Registers> StoreResult { get; }
        }

        public class DivideAOperation : DivideOperation
        {
            protected override Action<long, Registers> StoreResult { get; } = (val, mem) => mem.A = val;
        }

        public class BitwiseXorBOperation : LiteralOperation
        {
            protected override void ProcessInstruction(long operandValue,
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
            protected override void ProcessInstruction(long operandValue,
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
            protected override void ProcessInstruction(long operandValue, Registers memory, List<int> output, ref int instructionPointer)
            {
                if (memory.A == 0)
                {
                    return;
                }

                instructionPointer = (int)operandValue;
            }
        }

        public class BitwiseXorCOperation : LiteralOperation
        {
            protected override void ProcessInstruction(long operandValue, Registers memory, List<int> output, ref int instructionPointer)
            {
                var result = memory.B ^ memory.C;
                memory.B = result;
            }
        }

        public class OutOperation : ComboOperation
        {
            protected override void ProcessInstruction(long operandValue, Registers memory, List<int> output, ref int instructionPointer)
            {
                output.Add((int)(operandValue % 8));
            }
        }

        public class BDivisionOperation : DivideOperation
        {
            protected override Action<long, Registers> StoreResult { get; } = (val, mem) => mem.B = val;
        }

        public class CDivisionOperation : DivideOperation
        {
            protected override Action<long, Registers> StoreResult { get; } = (val, mem) => mem.C = val;
        }
    }
}
