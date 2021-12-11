using Helpers;
using Helpers.Extensions;
using Helpers.Maps;
using Helpers.Structure;

var solver = new Solver(new Day11Problem());

await solver.Solve();

class Day11Problem : ProblemBase
{
    private Cave cave;

    public override async Task ReadInput()
    {
        var octopi = await new OctopusReader().ReadInputFromFile();

        var octopiMap = octopi.To2DArray();

        cave = new Cave(octopiMap);
    }

    protected override Task<string> SolvePartOneInternal()
    {
        var numberOfTurns = 100;

        var numberOfFlashes = cave.CalculateNumberOfFlashes(numberOfTurns);

        return Task.FromResult(numberOfFlashes.ToString());
    }

    protected override Task<string> SolvePartTwoInternal()
    {
        throw new NotImplementedException();
    }
}

class OctopusReader : FileReader<Octopus[]>
{
    private int currentRow = 0;

    protected override Octopus[] ProcessLineOfFile(string line)
    {
        var energyLevels = line.ToCharArray()
                              .Select(x => int.Parse(x.ToString()))
                              .ToArray();

        var nextRow = new Octopus[energyLevels.Length];

        for (var x = 0; x < energyLevels.Length; x++)
        {
            nextRow[x] = new Octopus(new Coordinate(x,
                                                    currentRow))
                         {
                             EnergyLevel = energyLevels[x]
                         };
        }

        currentRow++;

        return nextRow;
    }
}

class Cave
{
    public Cave(Octopus[,] map)
    {
        CaveMap = map;
        AllOctopus = map.Cast<Octopus>()
                        .ToArray();
        NeighborMap = new Dictionary<Octopus, List<Octopus>>();
    }

    public Octopus[,] CaveMap { get; }

    public Octopus[] AllOctopus { get; }

    public Dictionary<Octopus, List<Octopus>> NeighborMap { get; }

    private List<Octopus> GetNeighbors(Octopus octopus)
    {
        if (NeighborMap.ContainsKey(octopus))
        {
            return NeighborMap[octopus];
        }

        var validNeighbors = octopus.Coordinate.GetNeighbors(true)
                                    .Where(coord => CaveMap.IsValidCoordinate(coord))
                                    .Select(coord => CaveMap[coord.X,
                                                             coord.Y])
                                    .ToList();

        NeighborMap[octopus] = validNeighbors;

        return validNeighbors;
    }

    public int CalculateNumberOfFlashes(int turnsToRun)
    {
        var totalFlashes = 0;

        for (var i = 0; i < turnsToRun; i++)
        {
            //CaveMap.Draw(x => x.EnergyLevel.ToString());

            totalFlashes += RunTurn();
        }

        return totalFlashes;
    }

    private const int FlashLevel = 9;

    private int RunTurn()
    {
        var flashedThisTurn = new HashSet<Octopus>();

        foreach (var octopus in AllOctopus)
        {
            octopus.EnergyLevel += 1;
        }

        while (true)
        {
            var flashed = AllOctopus.Where(x => x.EnergyLevel > FlashLevel && !flashedThisTurn.Contains(x))
                                    .ToList();

            if (flashed.Count == 0)
            {
                break;
            }

            flashed.ForEach(x =>
                            {
                                var neighbors = GetNeighbors(x);

                                neighbors.ForEach(x => x.EnergyLevel += 1);

                                flashedThisTurn.Add(x);
                            });
        }

        flashedThisTurn.ToList()
                       .ForEach(x => x.EnergyLevel = 0);

        return flashedThisTurn.Count;
    }
}

class Octopus : ObjectWithCoordinateEquality
{
    public int EnergyLevel { get; set; }

    public Octopus(Coordinate coordinate)
        : base(coordinate)
    {
    }
}