using System.Diagnostics;
using System.Text.RegularExpressions;

using Helpers;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day19 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var qualityLevelSum = 0;

            foreach (var blueprint in blueprints)
            {
                qualityLevelSum += FindMaxGeodesProduced(25,
                                                         blueprint)
                                   * blueprint.BlueprintNumber;
            }

            if (qualityLevelSum is < 1958 or > 1958)
            {
                throw new Exception("Wrong answer");
            }

            return qualityLevelSum.ToString();
        }

        private RobotType[] robotTypes =
        {
            RobotType.Geode,
            RobotType.Obsidian,
            RobotType.Clay,
            RobotType.Ore
        };

        private int FindMaxGeodesProduced(int endMinute,
                                          Blueprint blueprint)
        {
            var maxGeodes = 0;

            var stateQueue = new Queue<FactoryState>();

            var factory = new Factory(blueprint);

            var maxClayRobots = blueprint.ObsidianRobotCost.clay;

            var maxObsidianRobots = blueprint.GeodeRobotCost.obsidian;

            stateQueue.Enqueue(new FactoryState(1,
                                                0,
                                                0,
                                                0,
                                                0,
                                                0,
                                                0,
                                                1));

            var visited = new HashSet<FactoryState>();

            var testState = new FactoryState(1,
                                             4,
                                             2,
                                             3,
                                             13,
                                             8,
                                             0,
                                             18);

            while (stateQueue.Any())
            {
                var current = stateQueue.Dequeue();

                //if (current == testState)
                //{
                //    Debugger.Break();
                //}

                if (visited.Contains(current))
                {
                    // Already seen it
                    continue;
                }

                visited.Add(current);

                maxGeodes = Math.Max(current.Geodes,
                                     maxGeodes);

                var potentialMaximumFromThisState = current.Geodes + GetRemainingGeodesPossible(current.CurrentMinute);

                if (potentialMaximumFromThisState < maxGeodes)
                {
                    // We can end early if we can't beat the max we already found
                    continue;
                }

                var currentTimeRemaining = endMinute - current.CurrentMinute - 1;

                var updatedTime = current.CurrentMinute + 1;

                if (current.CurrentMinute == endMinute)
                {
                    // Time has ended
                    continue;
                }

                current = current.Update(timeRemaining: updatedTime);

                // Build every type of robot if possible
                foreach (var type in robotTypes)
                {
                    if (!factory.ShouldConstruct(type,
                                                 current,
                                                 currentTimeRemaining))
                    {
                        continue;
                    }

                    // If the most expensive robot is 5 ore for example
                    // We don't ever want more than 5 ore robots
                    // We can't use more than 5 per turn
                    if (type == RobotType.Ore
                        && current.OreRobots == factory.MaxOre)
                    {
                        continue;
                    }

                    if (type == RobotType.Clay
                        && current.ClayRobots == maxClayRobots)
                    {
                        continue;
                    }

                    if (type == RobotType.Obsidian
                        && current.ObsidianRobots == maxObsidianRobots)
                    {
                        continue;
                    }

                    var updated = factory.BeginConstruction(type,
                                                            current);
                    updated = factory.Mine(updated);
                    updated = factory.EndConstruction(type,
                                                      updated,
                                                      currentTimeRemaining);

                    stateQueue.Enqueue(updated);
                }

                // Simulate just mining this turn
                var miningOnly = factory.Mine(current);

                stateQueue.Enqueue(miningOnly);
            }

            return maxGeodes;
        }

        private Dictionary<int, int> cache = new();

        private int GetRemainingGeodesPossible(int minutes)
        {
            if (cache.TryGetValue(minutes,
                                  out var geodes))
            {
                return geodes;
            }

            if (minutes == 1)
            {
                return 1;
            }

            var result = GetRemainingGeodesPossible(minutes - 1) + minutes;

            cache[minutes] = result;

            return result;
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var maxOpened = 1;

            foreach (var blueprint in blueprints.Take(3))
            {
                maxOpened *= FindMaxGeodesProduced(33,
                                                   blueprint);
            }

            if (maxOpened is < 4257 or > 4257)
            {
                throw new Exception("Wrong answer");
            }

            return maxOpened.ToString();
        }

        public override async Task ReadInput()
        {
            blueprints = await new BlueprintReader().ReadInputFromFile();
        }

        private List<Blueprint> blueprints;
    }
}

public record FactoryState(int OreRobots,
                           int ClayRobots,
                           int ObsidianRobots,
                           int Ore,
                           int Clay,
                           int Obsidian,
                           int Geodes,
                           int CurrentMinute);

public static class StateExtensions
{
    public static FactoryState Update(this FactoryState state,
                                      int? oreRobots = null,
                                      int? clayRobots = null,
                                      int? obsidianRobots = null,
                                      int? ore = null,
                                      int? clay = null,
                                      int? obsidian = null,
                                      int? geodes = null,
                                      int? timeRemaining = null)
    {
        return new FactoryState(oreRobots ?? state.OreRobots,
                                clayRobots ?? state.ClayRobots,
                                obsidianRobots ?? state.ObsidianRobots,
                                ore ?? state.Ore,
                                clay ?? state.Clay,
                                obsidian ?? state.Obsidian,
                                geodes ?? state.Geodes,
                                timeRemaining ?? state.CurrentMinute);
    }
}

class Factory
{
    public Blueprint Blueprint { get; }

    public int MaxOre { get; }

    public FactoryState Mine(FactoryState state)
    {
        return state.Update(ore: state.Ore + state.OreRobots,
                            clay: state.Clay + state.ClayRobots,
                            obsidian: state.Obsidian + state.ObsidianRobots);
    }

    public FactoryState BeginConstruction(RobotType robotType, FactoryState factoryState)
    {
        var newOre = factoryState.Ore;
        var newClay = factoryState.Clay;
        var newObsidian = factoryState.Obsidian;

        switch (robotType)
        {
            case RobotType.Ore:
                newOre -= Blueprint.OreRobotCost;
                break;
            case RobotType.Clay:
                newOre -= Blueprint.ClayRobotCost;
                break;
            case RobotType.Obsidian:
                newOre -= Blueprint.ObsidianRobotCost.ore;
                newClay -= Blueprint.ObsidianRobotCost.clay;
                break;
            case RobotType.Geode:
                newOre -= Blueprint.GeodeRobotCost.ore;
                newObsidian -= Blueprint.GeodeRobotCost.obsidian;
                break;
            default:
                throw new ArgumentException(nameof(robotType));
        }

        return factoryState.Update(ore: newOre,
                                   clay: newClay,
                                   obsidian: newObsidian);
    }

    public FactoryState EndConstruction(RobotType robotType,
                                        FactoryState state,
                                        int minutesRemaining)
    {
        return robotType switch
        {
            RobotType.Ore => state.Update(oreRobots: state.OreRobots + 1),
            RobotType.Clay => state.Update(clayRobots: state.ClayRobots + 1),
            RobotType.Obsidian => state.Update(obsidianRobots: state.ObsidianRobots + 1),
            // For geodes, we don't need to track the robots, just track that we add one for each remaining minute
            RobotType.Geode => state.Update(geodes: state.Geodes + minutesRemaining),
            _ => throw new ArgumentOutOfRangeException(nameof(robotType),
                                                       robotType,
                                                       null)
        };
    }

    public bool ShouldConstruct(RobotType robotType, FactoryState state, int timeRemaining)
    {
        var materialCheck = robotType switch
        {
            RobotType.Ore => state.Ore >= Blueprint.OreRobotCost,
            RobotType.Clay => state.Ore >= Blueprint.ClayRobotCost,
            RobotType.Obsidian => state.Ore >= Blueprint.ObsidianRobotCost.ore && state.Clay >= Blueprint.ObsidianRobotCost.clay,
            RobotType.Geode => state.Ore >= Blueprint.GeodeRobotCost.ore && state.Obsidian >= Blueprint.GeodeRobotCost.obsidian,
            _ => throw new ArgumentException(nameof(robotType))
        };

        // We can't if we don't have the materials
        if (!materialCheck)
        {
            return false;
        }

        // We shouldn't if we never need the resources from it
        // X robots creating R, current stock Y, T minutes left, Z max requirement to build
        // X * T + Y >= T * Z
        // Amount we will mine in remaining time + stockpile >= the amount needed if we built that robot every remaining turn
        var robotIsUnnecessary = robotType switch
        {
            RobotType.Ore => (state.OreRobots * timeRemaining) + state.Ore >= (timeRemaining * MaxOre),
            RobotType.Clay => (state.ClayRobots * timeRemaining) + state.Clay >= (timeRemaining * Blueprint.ObsidianRobotCost.clay),
            RobotType.Obsidian => (state.ObsidianRobots * timeRemaining) + state.Obsidian >= (timeRemaining * Blueprint.GeodeRobotCost.obsidian),
            RobotType.Geode => false, // Always build geode robots
            _ => throw new ArgumentException(nameof(robotType))
        };

        return !robotIsUnnecessary;
    }

    public Factory(Blueprint blueprint)
    {
        Blueprint = blueprint;

        MaxOre = new[]
                 {
                     blueprint.OreRobotCost,
                     blueprint.ClayRobotCost,
                     blueprint.ObsidianRobotCost.ore,
                     blueprint.GeodeRobotCost.ore,
                 }.Max();
    }
}

enum RobotType
{
    Ore = 0,
    Clay = 1,
    Obsidian = 2,
    Geode = 3
}

enum ResourceType
{
    Ore = 0,
    Clay = 1,
    Obsidian = 2,
    Geode = 3
}

class Blueprint
{
    public int BlueprintNumber { get; set; }

    public int OreRobotCost { get; set; }

    public int ClayRobotCost { get; set; }

    public (int ore, int clay) ObsidianRobotCost { get; set; }

    public (int ore, int obsidian) GeodeRobotCost {get; set; }
}

class BlueprintReader : FileReader<Blueprint>
{
    private Regex lineRegex = new (@"Blueprint (\d+): Each ore robot costs (\d+) ore. Each clay robot costs (\d+) ore. Each obsidian robot costs (\d+) ore and (\d+) clay. Each geode robot costs (\d+) ore and (\d+) obsidian.");

    protected override Blueprint ProcessLineOfFile(string line)
    {
        var match = lineRegex.Match(line);

        int GetIntFromGroup(int groupIndex)
        {
            return int.Parse(match.Groups[groupIndex].Value);
        }

        return new Blueprint
               {
                   BlueprintNumber = GetIntFromGroup(1),
                   OreRobotCost = GetIntFromGroup(2),
                   ClayRobotCost = GetIntFromGroup(3),
                   ObsidianRobotCost = (GetIntFromGroup(4), GetIntFromGroup(5)),
                   GeodeRobotCost = (GetIntFromGroup(6), GetIntFromGroup(7))
               };
    }
}