using Helpers.FileReaders;
using Helpers.Structure;

var solver = new Solver(new Day24Problem());

await solver.Solve();

class Day24Problem : ProblemBase
{
    protected override async Task<string> SolvePartOneInternal()
    {
        // Runs slow because the answer starts w/ 3, so we churn through a lot of wrong checks
        var range = Enumerable.Range(1,
                                     9)
                              .Reverse()
                              .ToArray();

        var answer = Solve(new Dictionary<(long, int), long?>(),
                                  blocks,
                                  0,
                                  0,
                                  range);

        // Because we rebuild in reverse order we need to flip the string
        answer = long.Parse(new string(answer.ToString()
                                             .Reverse()
                                             .ToArray()));

        return answer.ToString();
    }

    private long? Solve(Dictionary<(long, int), long?> visited,
                               Instruction[][] instructionBlocks,
                               int blockIndex,
                               long zValue,
                               int[] inputRange)
    {
        // Uses a few properties of the instruction input:
        // X and Y are cleared after each input. W is always the input value. So we only really need to track Z and the "block" to track the state
        // And each block is more or less independent
        // So all we need to pass in from the previous is also the Z value

        // If we've seen this state before, short circuit
        if (visited.TryGetValue((zValue, blockIndex),
                                out var prevResult))
        {
            return prevResult;
        }

        foreach(var i in inputRange)
        {
            var alu = new ALU
                      {
                          Z = zValue
                      };

            // Run all the instructions in this block
            foreach (var instruction in instructionBlocks[blockIndex])
            {
                alu.RunInstruction(instruction,
                                   () => i);
            }

            // If this was the last instruction, check if z == 0 (aka we found a valid model number)
            if (blockIndex == instructionBlocks.Length - 1)
            {
                if (alu.Z == 0)
                {
                    visited.Add((alu.Z, blockIndex),
                                i);
                    return i;
                }

                // Otherwise, continue, don't want to recurse past this final digit
                continue;
            }

            // Recurse on the next digit with 'i' as the current digit
            var result = Solve(visited,
                               instructionBlocks,
                               blockIndex + 1,
                               alu.Z,
                               inputRange);

            // If it didn't return null, we found a solution and can start building up the actual value in reverse4
            // This is easier than trying to figure out what the digits up the call stack are
            if (result.HasValue)
            {
                var thisResult = result * 10 + i;

                visited.Add((alu.Z, blockIndex),
                            thisResult);
                return thisResult;
            }
        }

        // Looped through all options, so add null. None of the digit values for i will work with the parent digit value.
        visited.Add((zValue, blockIndex),
                    null);
        return null;
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        // Same as PT 1 just try digit 1 first
        var range = Enumerable.Range(1,
                                     9)
                              .ToArray();

        var answer = Solve(new Dictionary<(long, int), long?>(),
                           blocks,
                           0,
                           0,
                           range);

        // Because we rebuild in reverse order we need to flip the string
        answer = long.Parse(new string(answer.ToString()
                                             .Reverse()
                                             .ToArray()));

        return answer.ToString();
    }

    public override async Task ReadInput()
    {
        var instructionStrings = (await new StringFileReader().ReadInputFromFile()).ToArray();

        blocks = new Instruction[14][];

        var blockIdx = 0;

        var blockArrayIdx = 0;

        foreach (var instructionString in instructionStrings)
        {
            var split = instructionString.Split(' ');

            var type = split[0] switch
            {
                "inp" => InstructionType.Input,
                "add" => InstructionType.Add,
                "mul" => InstructionType.Mul,
                "div" => InstructionType.Div,
                "mod" => InstructionType.Mod,
                "eql" => InstructionType.Eql,
                _ => throw new ArgumentOutOfRangeException(split[0])
            };

            Func<ALU, long> getZ = alu => alu.Z;
            Func<ALU, long> getX = alu => alu.X;
            Func<ALU, long> getY = alu => alu.Y;
            Func<ALU, long> getW = alu => alu.W;

            Action<long, ALU> setZ = (val, alu) => alu.Z = val;
            Action<long, ALU> setX = (val, alu) => alu.X = val;
            Action<long, ALU> setY = (val, alu) => alu.Y = val;
            Action<long, ALU> setW = (val, alu) => alu.W = val;


            var getDest = split[1] switch
            {
                "x" => getX,
                "y" => getY,
                "w" => getW,
                "z" => getZ,
                _ => throw new ArgumentOutOfRangeException(split[1])
            };

            var setDest = split[1] switch
            {
                "x" => setX,
                "y" => setY,
                "w" => setW,
                "z" => setZ,
                _ => throw new ArgumentOutOfRangeException(split[1])
            };

            var hardCodedValue = 0L;

            if (type != InstructionType.Input)
            {
                long.TryParse(split[2],
                              out hardCodedValue);
            }
            
            var instruction = new Instruction
                              {
                                  Type = type,
                                  GetDestination = getDest,
                                  SetDestination = setDest,
                                  GetRightSide = type == InstructionType.Input
                                                     ? null
                                                     : split[2] switch
                                                     {
                                                         "x" => alu => alu.X,
                                                         "y" => alu => alu.Y,
                                                         "w" => alu => alu.W,
                                                         "z" => alu => alu.Z,
                                                         _ => _ => hardCodedValue
                                                     }
                              };

            if (instruction.Type == InstructionType.Input)
            {
                blocks[blockIdx] = new Instruction[18];
                blockIdx++;
                blockArrayIdx = 0;
            }

            blocks[blockIdx - 1][blockArrayIdx++] = instruction;
        }
    }

    private Instruction[][] blocks;
}

class Instruction
{
    public InstructionType Type { get; set; }

    public Action<long, ALU> SetDestination { get; set; }

    public Func<ALU, long> GetDestination { get; set; }

    public Func<ALU, long>? GetRightSide { get; set; }
}

enum InstructionType
{
    Input,
    Add,
    Mul,
    Div,
    Mod,
    Eql
}

class ALU
{
    public long X { get; set; }

    public long Y { get; set; }

    public long Z { get; set; }

    public long W { get; set; }

    public void RunInstruction(Instruction instruction,
                               Func<int> getInput)
    {
        long a;
        long b;

        switch (instruction.Type)
        {
            case InstructionType.Input:
                var input = getInput();

                instruction.SetDestination(input,
                                           this);
                break;
            case InstructionType.Add:
                a = instruction.GetDestination(this);

                b = instruction.GetRightSide!(this);

                instruction.SetDestination(a + b,
                                           this);
                break;
            case InstructionType.Mul:
                a = instruction.GetDestination(this);

                b = instruction.GetRightSide!(this);

                instruction.SetDestination(a * b,
                                           this);
                break;
            case InstructionType.Div:
                a = instruction.GetDestination(this);

                b = instruction.GetRightSide!(this);

                instruction.SetDestination(a / b,
                                           this);
                break;
            case InstructionType.Mod:
                a = instruction.GetDestination(this);

                b = instruction.GetRightSide!(this);

                instruction.SetDestination(a % b,
                                           this);
                break;
            case InstructionType.Eql:
                a = instruction.GetDestination(this);

                b = instruction.GetRightSide!(this);

                instruction.SetDestination(a == b
                                               ? 1
                                               : 0,
                                           this);
                break;
        }
    }
}