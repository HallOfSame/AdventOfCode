using System.Text.RegularExpressions;

using Helpers.FileReaders;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day05 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            foreach (var move in moves)
            {
                var source = stacks[move.SourceStackIndex];
                var dest = stacks[move.DestinationStackIndex];

                for (var i = 0; i < move.CrateCount; i++)
                {
                    var crateToMove = source.Remove();
                    dest.Add(crateToMove);
                }
            }

            return new string(stacks.Select(x => x.Remove()).ToArray());
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            foreach (var move in moves)
            {
                var source = stacksForPartTwo[move.SourceStackIndex];
                var dest = stacksForPartTwo[move.DestinationStackIndex];

                var cratesToMove = source.Remove(move.CrateCount);
                dest.Add(cratesToMove);
            }

            return new string(stacksForPartTwo.Select(x => x.Remove(1).First()).ToArray());
        }

        public override async Task ReadInput()
        {
            var fileData = await new StringFileReader().ReadInputFromFile();

            var numberOfStacks = fileData.First()
                                         .Length
                                 / 4
                                 + 1;

            var crateStacks = Enumerable.Range(0,
                                               numberOfStacks)
                                        .Select(x => new List<char>())
                                        .ToArray();

            // Read initial setup
            foreach (var line in fileData)
            {
                if (!line.Contains("["))
                {
                    break;
                }

                for (var i = 0; i < numberOfStacks; i++)
                {
                    var crateAtIndex = line.Substring(i * 4,
                                                      3);

                    if (string.IsNullOrWhiteSpace(crateAtIndex))
                    {
                        continue;
                    }

                    crateStacks[i]
                        .Add(crateAtIndex[1]);
                }
            }

            stacks = crateStacks.Select(x =>
                                        {
                                            x.Reverse();

                                            var stack = new CrateStack();

                                            x.ForEach(ch => stack.Add(ch));

                                            return stack;
                                        })
                                .ToArray();

            stacksForPartTwo = crateStacks.Select(x =>
                                                  {
                                                      // No reverse here, the one from above altered the list in place 

                                                      var stack = new CrateStackv2();

                                                      stack.Add(x);

                                                      return stack;
                                                  })
                                          .ToArray();

            // Read in moves
            // Skip the lines that are the initial stacks + the one showing the stack number + the empty line
            var skip = crateStacks.Max(x => x.Count) + 2;

            var moveRegex = new Regex(@"move (\d+) from (\d+) to (\d)");

            var moveList = new List<CrateMove>();

            foreach (var line in fileData.Skip(skip))
            {
                var groups = moveRegex.Match(line).Groups;

                moveList.Add(new CrateMove
                             {
                                 CrateCount = int.Parse(groups[1]
                                                            .Value),
                                 SourceStackIndex = int.Parse(groups[2]
                                                                  .Value) - 1,
                                 DestinationStackIndex = int.Parse(groups[3]
                                                                       .Value) - 1
                             });
            }

            moves = moveList.ToArray();
        }

        private CrateStack[] stacks;

        private CrateStackv2[] stacksForPartTwo;

        private CrateMove[] moves;
    }
}

class CrateStack
{
    private readonly Stack<char> innerStack = new();

    public char Remove()
    {
        return innerStack.Pop();
    }

    public void Add(char c)
    {
        innerStack.Push(c);
    }
}

class CrateStackv2
{
    private readonly Stack<char> innerStack = new();

    public List<char> Remove(int count)
    {
        var ret = new List<char>(count);

        for (var i = 0; i < count; i++)
        {
            ret.Add(innerStack.Pop());
        }

        ret.Reverse();

        return ret;
    }

    public void Add(IEnumerable<char> characters)
    {
        foreach (var c in characters)
        {
            innerStack.Push(c);
        }
    }
}

class CrateMove
{
    public int SourceStackIndex { get; init; }

    public int DestinationStackIndex { get; init; }

    public int CrateCount { get; init; }
}