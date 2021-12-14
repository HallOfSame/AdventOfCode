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

        return Task.FromResult(CalculatePuzzleAnswer());
    }

    private string CalculatePuzzleAnswer()
    {
        var currentCharArrayCounts = currentPolymer.ToCharArray()
                                                   .GroupBy(x => x)
                                                   .Select(x => new
                                                                {
                                                                    Char = x.Key,
                                                                    Count = x.LongCount()
                                                                })
                                                   .ToList();

        var minOccurrence = currentCharArrayCounts.OrderBy(x => x.Count)
                                                  .First()
                                                  .Count;
        var maxOccurrence = currentCharArrayCounts.OrderByDescending(x => x.Count)
                                                  .First()
                                                  .Count;

        return (maxOccurrence - minOccurrence).ToString();
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

    protected override Task<string> SolvePartTwoInternal()
    {
        // Key is the pair i.e. "NN" and value is the number of times it currently occurs
        var pairCounts = new Dictionary<string, long>();

        for (var i = 0; i < template.Length -1; i++)
        {
            var pair = template.Substring(i,
                                          2);

            if (!pairCounts.ContainsKey(pair))
            {
                pairCounts[pair] = 1;
            }
            else
            {
                pairCounts[pair] += 1;
            }
        }

        var numberOfSteps = 40;

        // Stores what pairs increase what other pairs so we don't have to do the sub string calc every time
        var pairMatches = new Dictionary<string, (string one, string two)>();

        // Stores the actual character amounts by step
        var charCount = template.ToCharArray()
                                .GroupBy(x => x)
                                .ToDictionary(x => x.Key,
                                              x => x.LongCount());

        for (var i = 0; i < numberOfSteps; i++)
        {
            // Every pair that has a count > 0 increases the count of 2 other pairs by the number of instances it occurs
            foreach (var pair in pairCounts.ToList())
            {
                var replacement = replacements[pair.Key];

                if (!pairMatches.TryGetValue(pair.Key,
                                             out var matches))
                {
                    var addedPairOne = pair.Key.Substring(0,
                                                          1)
                                       + replacement;

                    var addedPairTwo = replacement
                                       + pair.Key.Substring(1,
                                                            1);

                    pairMatches[pair.Key] = (addedPairOne, addedPairTwo);
                    matches = (addedPairOne, addedPairTwo);
                }

                // For each occurrence of this pair, we added a new replacement[0] char to the string
                // So update the count
                if (!charCount.ContainsKey(replacement[0]))
                {
                    charCount[replacement[0]] = pair.Value;
                }
                else
                {
                    charCount[replacement[0]] += pair.Value;
                }

                // We also need to track the pairs so that we can keep updating counts
                if (!pairCounts.ContainsKey(matches.one))
                {
                    pairCounts[matches.one] = pair.Value;
                }
                else
                {
                    pairCounts[matches.one] += pair.Value;
                }

                if (!pairCounts.ContainsKey(matches.two))
                {
                    pairCounts[matches.two] = pair.Value;
                }
                else
                {
                    pairCounts[matches.two] += pair.Value;
                }

                // The existing pairs all got broken up
                pairCounts[pair.Key] -= pair.Value;
            }
        }

        var maxRepeated = charCount.MaxBy(x => x.Value);

        var maxCount = maxRepeated.Value;

        var minRepeated = charCount.MinBy(x => x.Value);

        var minCount = minRepeated.Value;

        return Task.FromResult((maxCount - minCount).ToString());
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