using Helpers.FileReaders;
using Helpers.Structure;

var solver = new Solver(new Day03Problem());

await solver.Solve();

class Day03Problem : ProblemBase
{
    protected override async Task<string> SolvePartOneInternal()
    {
        var sum = 0;

        foreach (var sack in sacks)
        {
            var first = sack.Item1.ToHashSet();
            var second = sack.Item2.ToHashSet();

            first.IntersectWith(second);

            if (first.Count != 1)
            {
                throw new InvalidOperationException("Did not have exactly 1 overlap in sack.");
            }

            sum += CharToPriority(first.First());
        }

        return sum.ToString();
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        var groups = sacks.Count / 3;

        var sum = 0;

        for (var i = 0; i < groups; i++)
        {
            sum += CharToPriority(fullSacks.Skip(i * 3)
                                           .Take(3)
                                           .Aggregate(new HashSet<char>(),
                                                      (currentSet,
                                                       nextLine) =>
                                                      {
                                                          // Seed with first line
                                                          if (currentSet.Count == 0)
                                                          {
                                                              currentSet = new HashSet<char>(nextLine.ToCharArray());
                                                              return currentSet;
                                                          }

                                                          // Intersect with remaining elves
                                                          currentSet.IntersectWith(new HashSet<char>(nextLine.ToCharArray()));

                                                          return currentSet;
                                                      })
                                           .First());
        }

        return sum.ToString();
    }

    public override async Task ReadInput()
    {
        fullSacks = await new StringFileReader().ReadInputFromFile();

        sacks = fullSacks.Select(line =>
                                 {
                                     var length = line.Length;

                                     return (line.Substring(0,
                                                            length / 2), line.Substring(length / 2));
                                 })
                         .ToList();
    }

    private List<string> fullSacks;

    private List<(string, string)> sacks;

    private int CharToPriority(char input)
    {
        var value = input - 96;

        if (value < 0)
        {
            value += 58;
        }

        return value;
    }
}