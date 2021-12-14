using Helpers.FileReaders;
using Helpers.Structure;

var solver = new Solver(new Day14Problem());

await solver.Solve();

class Day14Problem : ProblemBase
{
    protected override Task<string> SolvePartOneInternal()
    {
        currentPolymer = template;

        var numberOfIterations = 10;

        for (var i = 0; i < numberOfIterations; i++)
        {
            RunOneIteration();
        }

        var currentCharArrayCounts = currentPolymer.ToCharArray()
                                                   .GroupBy(x => x)
                                                   .Select(x => new
                                                                {
                                                                    Char = x.Key,
                                                                    Count = x.Count()
                                                                })
                                                   .ToList();

        var minOccurrence = currentCharArrayCounts.OrderBy(x => x.Count)
                                                  .First()
                                                  .Count;
        var maxOccurrence = currentCharArrayCounts.OrderByDescending(x => x.Count)
                                                  .First()
                                                  .Count;

        return Task.FromResult((maxOccurrence - minOccurrence).ToString());
    }

    private string currentPolymer;

    private void RunOneIteration()
    {
        var currentCharArray = currentPolymer.ToCharArray();

        var updatedPolymer = currentPolymer;
        
        // Keeps the offset needed because the updated string keeps getting longer as we process
        var replacementOffset = 0;

        for (var i = 0; i < currentCharArray.Length; i++)
        {
            var currentPair = new string(currentCharArray.Skip(i)
                                                         .Take(2)
                                                         .ToArray());

            if (!replacements.TryGetValue(currentPair,
                                          out var replacement))
            {
                // Not sure this is actually possibly in the input or not
                continue;
            }

            updatedPolymer = updatedPolymer.Insert(i + 1 + replacementOffset,
                                                   replacement);

            replacementOffset++;
        }

        currentPolymer = updatedPolymer;
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        throw new NotImplementedException();
    }

    public override async Task ReadInput()
    {
        var strings = await new StringFileReader().ReadInputFromFile();

        template = strings.First();

        foreach (var s in strings.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                continue;
            }

            var split = s.Split(" -> ");

            replacements.Add(split[0],
                             split[1]);
        }
    }

    private string template;

    private readonly Dictionary<string, string> replacements = new();
}