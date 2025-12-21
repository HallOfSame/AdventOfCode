using Helpers.Structure;

namespace PuzzleDays;

public class Day12 : SingleExecutionPuzzle<Day12.State>
{
    public override PuzzleInfo Info => new(2025, 12, "Christmas Tree Farm");

    protected override async Task<string> ExecutePuzzlePartOne()
    {
        var result = 0;

        foreach (var tree in InitialState.Trees)
        {
            if (CanFitAllPresents(tree))
            {
                result += 1;
            }
        }

        return result.ToString();
    }

    private static bool CanFitAllPresents(Tree tree)
    {
        // How many presents we need, multiplied by 9 since the footprints are all 3x3
        var countRequired = tree.PresentsRequired.Sum() * 9;

        var nonOverlappingSpace = tree.Width * tree.Height;

        return nonOverlappingSpace >= countRequired;
    }

    protected override async Task<string> ExecutePuzzlePartTwo()
    {
        throw new NotImplementedException();
    }

    protected override async Task<State> LoadInputState(string puzzleInput, PuzzleInputType inputType)
    {
        // Just remove the present shapes from the top
        var lines = puzzleInput.Split('\n')
            .ToList();
        var trees = new List<Tree>();
        var treeIndex = 0;

        foreach (var tree in lines)
        {
            var lineSplit = tree.Split(':');
            var whSplit = lineSplit[0]
                .Split('x');
            var width = int.Parse(whSplit[0]);
            var height = int.Parse(whSplit[1]);
            var requirements = lineSplit[1]
                .Trim()
                .Split(' ')
                .Select(int.Parse)
                .ToArray();

            trees.Add(new Tree
            {
                Index = treeIndex++,
                Width = width,
                Height = height,
                PresentsRequired = requirements
            });
        }

        return new State
        {
            Trees = trees
        };
    }

    public class State
    {
        public required List<Tree> Trees { get; init; }
    }

    public class Tree
    {
        public required int Index { get; init; }
        public required int Width { get; init; }
        public required int Height { get; init; }
        public required int[] PresentsRequired { get; init; }
    }
}