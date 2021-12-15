using Helpers;
using Helpers.Extensions;
using Helpers.Maps;
using Helpers.Structure;

var solver = new Solver(new Day15Problem());

await solver.Solve();

class Day15Problem : ProblemBase
{
    private List<CaveLocation[]> locations;

    private Cave cave;

    protected override Task<string> SolvePartOneInternal()
    {
        return Task.FromResult(cave.GetShortestPathLength()
                                   .ToString());
    }

    protected override Task<string> SolvePartTwoInternal()
    {
        var xMod = locations[0].Length;
        var yMod = locations.Count;

        var locations2d = locations.To2DArray();

        var bigCaveLocations = new CaveLocation[xMod * 5, yMod * 5];

        for (var yTile = 0; yTile < 5; yTile++)
        {
            for (var xTile = 0; xTile < 5; xTile++)
            {
                // There is probably a better way to do this
                for (var y = 0; y < yMod; y++)
                {
                    for (var x = 0; x < xMod; x++)
                    {
                        var originalCoordinate = locations2d[x,
                                                             y];

                        var updatedRiskLevel = originalCoordinate.RiskLevel + yTile + xTile;

                        if (updatedRiskLevel > 9)
                        {
                            updatedRiskLevel -= 9;
                        }

                        var largerX = (xMod * xTile) + x;
                        var largerY = (yMod * yTile) + y;

                        bigCaveLocations[largerX,
                                         largerY] = new CaveLocation(new Coordinate(largerX,
                                                                                    largerY))
                                                    {
                                                        RiskLevel = updatedRiskLevel
                                                    };
                    }
                }
            }
        }

        var bigCave = new Cave(bigCaveLocations);

        return Task.FromResult(bigCave.GetShortestPathLength()
                                      .ToString());
    }

    public override async Task ReadInput()
    {
        locations = await new LocationReader().ReadInputFromFile();

        cave = new Cave(locations.To2DArray());
    }
}

class Cave
{
    private readonly CaveLocation[,] caveLocations;

    private int height;

    private int width;

    public Cave(CaveLocation[,] locations)
    {
        this.caveLocations = locations;
        this.height = locations.GetLength(0);
        this.width = locations.GetLength(1);
    }

    public int GetShortestPathLength()
    {
        var endLocation = caveLocations[height - 1,
                                        width - 1];

        // This is more or less an implementation of Dijkstra's alg
        var distanceToGoal = caveLocations.Cast<CaveLocation>()
                                          .ToDictionary(x => x,
                                                        x => int.MaxValue);

        distanceToGoal[endLocation] = endLocation.RiskLevel;

        var notVisited = new PriorityQueue<CaveLocation, int>(distanceToGoal.Count);

        notVisited.Enqueue(endLocation,
                           endLocation.RiskLevel);

        while (notVisited.Count != 0)
        {
            var current = notVisited.Dequeue();

            foreach (var neighborCoordinate in current.Coordinate.GetNeighbors())
            {
                var neighborLocation = caveLocations.IsValidCoordinate(neighborCoordinate)
                                           ? caveLocations[neighborCoordinate.X,
                                                           neighborCoordinate.Y]
                                           : null;

                if (neighborLocation == null)
                {
                    continue;
                }

                var neighborDistance = distanceToGoal[current] + neighborLocation.RiskLevel;

                if (neighborDistance < distanceToGoal[neighborLocation])
                {
                    distanceToGoal[neighborLocation] = neighborDistance;
                    notVisited.Enqueue(neighborLocation,
                                       neighborDistance);
                }
            }
        }

        // Subtract the risk level of the start since it doesn't count for the puzzle
        return distanceToGoal[caveLocations[0,
                                            0]]
               - caveLocations[0,
                               0]
                   .RiskLevel;
    }
}

class CaveLocation : ObjectWithCoordinateEquality
{
    public int RiskLevel { get; init; }

    public CaveLocation(Coordinate coordinate)
        : base(coordinate)
    {
    }
}

class LocationReader : FileReader<CaveLocation[]>
{
    private int currentRow = 0;

    protected override CaveLocation[] ProcessLineOfFile(string line)
    {
        var locations = line.ToCharArray()
                            .Select((x,
                                     idx) => new CaveLocation(new Coordinate(idx,
                                                                             currentRow))
                                             {
                                                 RiskLevel = int.Parse(x.ToString())
                                             })
                            .ToArray();

        currentRow++;

        return locations;
    }
}