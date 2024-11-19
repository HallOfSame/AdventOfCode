using System.Collections.Generic;
using System.Linq;
using Helpers.Interfaces;

namespace Helpers.Structure;

public class PuzzleContainer(IEnumerable<IPuzzle> puzzles) : IPuzzleContainer
{
    public IPuzzle GetPuzzle(int year, int day)
    {
        return puzzles.Single(x => x.Info.Year == year && x.Info.Day == day);
    }
}