using Helpers;
using Helpers.Extensions;
using Helpers.Maps;
using Helpers.Structure;

var solver = new Solver(new Day15Problem());

await solver.Solve();

class Day15Problem : ProblemBase
{
    private Cave cave;

    protected override Task<string> SolvePartOneInternal()
    {
        return Task.FromResult(cave.GetShortestPathLength()
                                   .ToString());
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        throw new NotImplementedException();
    }

    public override async Task ReadInput()
    {
        var locations = await new LocationReader().ReadInputFromFile();

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

        int ManhattanDistanceToEnd(Coordinate current)
        {
            return Math.Abs(current.X - endLocation.Coordinate.X) + Math.Abs(current.Y - endLocation.Coordinate.Y);
        }

        var start = caveLocations[0,
                                  0];

        var workingSet = new HashSet<CaveLocation>
                         {
                             start
                         };

        // cameFromMap[x] == the cheapest coordinate to reach it from the start location
        // Can be used to rebuild the pathway
        var cameFromMap = new Dictionary<CaveLocation, CaveLocation>();

        // gScoreMap[x] == the cost of the cheapest path from start to x
        var gScoreMap = caveLocations.Cast<CaveLocation>()
                                     .ToDictionary(x => x.Coordinate,
                                                   x => int.MaxValue);

        // fScoreMap[x] == our guess to how far x is from the start, using gScore + a guesstimate
        var fScoreMap = gScoreMap.ToDictionary(x => x.Key,
                                               x => x.Value);

        gScoreMap[start.Coordinate] = 0;
        fScoreMap[start.Coordinate] = ManhattanDistanceToEnd(start.Coordinate);

        while (workingSet.Any())
        {
            var current = workingSet.OrderBy(x => fScoreMap[x.Coordinate])
                                    .First();

            if (Equals(current,
                       endLocation))
            {
                // We reached the O2 system, return the number of moves needed
                return gScoreMap[current.Coordinate];
            }

            workingSet.Remove(current);

            foreach (var neighbor in current.Coordinate.GetNeighbors()
                         .Where(x => caveLocations.IsValidCoordinate(x)))
            {
                var neighborLocation = caveLocations[neighbor.X,
                                                     neighbor.Y];

                var tentativeGScore = gScoreMap[current.Coordinate] + neighborLocation.RiskLevel;

                // This path is better, so record it
                if (tentativeGScore < gScoreMap[neighbor])
                {
                    // Record current best way to reach neighbor
                    cameFromMap[neighborLocation] = current;
                    // Update the scores
                    gScoreMap[neighbor] = tentativeGScore;
                    fScoreMap[neighbor] = tentativeGScore + ManhattanDistanceToEnd(neighbor);
                    // Ensure neighbor is in the working set again
                    workingSet.Add(neighborLocation);
                }
            }
        }

        throw new Exception("Somehow couldn't find any valid path.");
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