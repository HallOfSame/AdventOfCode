using Helpers;
using Helpers.Structure;

namespace PuzzleDays;

public class Day04 : ProblemBase
{
    protected override async Task<string> SolvePartOneInternal()
    {
        return cleaningPairs.Count(x => x.FullOverlap())
                            .ToString();
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        return cleaningPairs.Count(x => x.AnyOverlap())
                            .ToString();
    }

    public override async Task ReadInput()
    {
        cleaningPairs = await new PairReader().ReadInputFromFile();
    }

    private List<CleaningPair> cleaningPairs;
}

class CleaningPair
{
    public Range Earlier { get; init; }

    public Range Later { get; init; }

    public bool FullOverlap()
    {
        return (Earlier.Lower <= Later.Lower && Earlier.Upper >= Later.Upper) || (Later.Lower <= Earlier.Lower && Later.Upper >= Earlier.Upper);
    }

    public bool AnyOverlap()
    {
        return Earlier.Upper >= Later.Lower;
    }
}

class Range
{
    public int Lower { get; init; }

    public int Upper { get; init; }
}

class PairReader : FileReader<CleaningPair>
{
    protected override CleaningPair ProcessLineOfFile(string line)
    {
        var pairs = line.Split(',');

        var ranges = pairs.Select(p => p.Split('-'))
                          .Select(p => p.Select(int.Parse))
                          .Select(x => new Range
                                       {
                                           Lower = x.First(),
                                           Upper = x.Last()
                                       })
                          .OrderBy(x => x.Lower)
                          .ThenBy(x => x.Upper);

        return new CleaningPair
               {
                   Earlier = ranges.First(),
                   Later = ranges.Last()
               };
    }
}