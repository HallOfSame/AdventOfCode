using System.Text;

using Helpers;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day10 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var checkCycles = new HashSet<int>
                              {
                                  20,
                                  60,
                                  100,
                                  140,
                                  180,
                                  220
                              };

            processor = new Processor(240);

            processor.Run(instructions);

            return checkCycles.Select(x => processor.xHistory[x - 1] * x)
                              .Sum()
                              .ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var sb = new StringBuilder();

            for (var row = 0; row < 6; row++)
            {
                for (var col = 1; col < 41; col++)
                {
                    var cycle = (row * 40) + col;

                    var position = col - 1;

                    var xCenterColAtThisCycle = processor.xHistory[cycle - 1];

                    if (Math.Abs(xCenterColAtThisCycle - position) <= 1)
                    {
                        sb.Append("#");
                    }
                    else
                    {
                        sb.Append(".");
                    }
                }

                sb.Append(Environment.NewLine);
            }

            Console.WriteLine(sb.ToString());

            return "FZBPBFZF";
        }

        public override async Task ReadInput()
        {
            instructions = await new InstructionReader().ReadInputFromFile();
        }

        private List<Instruction> instructions;

        private Processor processor;
    }
}

class Processor
{
    private readonly int maxCycles;

    public int XRegister { get; set; } = 1;

    public int InstructionPointer { get; set; }

    public int[] xHistory { get; set; }

    public Processor(int maxCycles)
    {
        this.maxCycles = maxCycles;
        xHistory = new int[maxCycles];
        xHistory[0] = 1;
    }

    public void Run(List<Instruction> instructions)
    {
        // When true we should process the add instruction in this cycle
        var performAdd = false;

        for (var cycle = 1; cycle < maxCycles; cycle++)
        {
            var instruction = instructions[InstructionPointer];

            if (instruction.InstructionType == InstructionType.Add)
            {
                // Flip the flag
                performAdd = !performAdd;
            }

            // Save the history during the instruction
            xHistory[cycle] = XRegister;

            if (instruction.InstructionType == InstructionType.Add && performAdd)
            {
                InstructionPointer++;
                XRegister += instruction.Value;
            }
            else if (instruction.InstructionType == InstructionType.Noop)
            {
                InstructionPointer++;
            }
        }
    }
}

enum InstructionType
{
    Add,
    Noop
}

class Instruction
{
    public InstructionType InstructionType { get; set; }

    public int Value { get; set; }
}

class InstructionReader : FileReader<Instruction>
{
    protected override Instruction ProcessLineOfFile(string line)
    {
        if (line.Equals("noop"))
        {
            return new Instruction
                   {
                       InstructionType = InstructionType.Noop
                   };
        }

        var split = line.Split(' ');

        var value = int.Parse(split[1]);

        return new Instruction
               {
                   InstructionType = InstructionType.Add,
                   Value = value
               };
    }
}