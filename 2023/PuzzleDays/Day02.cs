using System.Text.RegularExpressions;
using Helpers;
using Helpers.Structure;

namespace PuzzleDays;

public class Day02 : ProblemBase
{
    private List<GameResults> gameResults { get; set; }

    protected override async Task<string> SolvePartOneInternal()
    {
        var redLimit = 12;
        var blueLimit = 14;
        var greenLimit = 13;

        var result = gameResults.Where(game => game.RevealedSets.All(set => set.RedCount <= redLimit &&
                                                                            set.BlueCount <= blueLimit &&
                                                                            set.GreenCount <= greenLimit))
            .Select(game => game.GameId).Sum();

        return result.ToString();
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        var totalSum = 0m;

        foreach (var result in gameResults)
        {
            var redNeeded = result.RevealedSets.Max(set => set.RedCount);
            var blueNeeded = result.RevealedSets.Max(set => set.BlueCount);
            var greenNeeded = result.RevealedSets.Max(set => set.GreenCount);

            var power = redNeeded * blueNeeded * greenNeeded;

            totalSum += power;
        }

        return totalSum.ToString();
    }

    public override async Task ReadInput()
    {
        var reader = new GameResultReader();

        gameResults = await reader.ReadInputFromFile();
    }
}

internal class GameResults
{
    public int GameId { get; set; }
    public List<CubeSet> RevealedSets { get; set; }
}

internal class CubeSet
{
    public int RedCount { get; set; }
    public int BlueCount { get; set; }
    public int GreenCount { get; set; }
}

internal class GameResultReader : FileReader<GameResults>
{
    protected override GameResults ProcessLineOfFile(string line)
    {
        var initialSplit = line.Split(':');

        var gameId = int.Parse(initialSplit[0].Replace("Game ", string.Empty));

        var cubeResultSplits = initialSplit[1].Split(';');

        var cubeSets = new List<CubeSet>();

        foreach (var cubeSplit in cubeResultSplits)
        {
            var thisCubeShowing = new CubeSet();

            var cubesInThisResult = cubeSplit.Split(',');

            foreach (var match in cubesInThisResult)
            {
                var splitMatch = match.Trim().Split(' ');
                var count = int.Parse(splitMatch[0]);
                var color = splitMatch[1];

                switch (color)
                {
                    case "red":
                        thisCubeShowing.RedCount = count;
                        break;
                    case "blue":
                        thisCubeShowing.BlueCount = count;
                        break;
                    case "green":
                        thisCubeShowing.GreenCount = count;
                        break;
                    default:
                        throw new Exception($"{color} is not expected");
                }
            }

            cubeSets.Add(thisCubeShowing);
        }

        return new GameResults
        {
            GameId = gameId,
            RevealedSets = cubeSets
        };
    }
}