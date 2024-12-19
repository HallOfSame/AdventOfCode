using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;
using InputStorageDatabase;

namespace PuzzleDays;

public class Day12 : SingleExecutionPuzzle<Day12.ExecState>
{
    public record ExecState(Dictionary<Coordinate, char> GardenMap);

    public override PuzzleInfo Info => new(2024, 12, "Garden Groups");

    protected override async Task<ExecState> LoadInputState(string puzzleInput, PuzzleInputType inputType)
    {
        var grid = await new GridFileReader().ReadFromString(puzzleInput.Trim());
        
        return new ExecState(grid.ToDictionary(x => x.Coordinate, x => x.Value));
    }

    private List<Region> regions = [];

    protected override async Task<string> ExecutePuzzlePartOne()
    {
        var visited = new HashSet<Coordinate>();
        var nextStartingPoint = InitialState.GardenMap.First();
        regions.Clear();

        do
        {
            var newRegion = ExploreRegion(nextStartingPoint);
            regions.Add(newRegion);
            visited.UnionWith(newRegion.Coordinates);

            nextStartingPoint = InitialState.GardenMap.FirstOrDefault(x => !visited.Contains(x.Key));
        }
        while (visited.Count != InitialState.GardenMap.Count);

        var result = regions.Sum(r => r.GetArea() * r.GetPerimeter());
        return result.ToString();
    }

    protected override async Task<string> ExecutePuzzlePartTwo()
    {
        if (regions.Count == 0)
        {
            throw new InvalidOperationException("Run part 1 first");
        }
        
        var result = regions.Sum(r => r.GetArea() * r.GetSides(InitialState.GardenMap));
        return result.ToString();
    }

    private Region ExploreRegion(KeyValuePair<Coordinate, char> startingPoint)
    {
        var queue = new Queue<Coordinate>();
        queue.Enqueue(startingPoint.Key);
        var newRegion = new Region
                        {
                            RegionId = startingPoint.Value,
                            Coordinates = [startingPoint.Key]
                        };

        while (queue.Count != 0)
        {
            var next = queue.Dequeue();

            var neighbors = next.GetNeighbors()
                                .Where(x => InitialState.GardenMap.TryGetValue(x, out var neighborId) && neighborId == startingPoint.Value)
                                .ToList();

            foreach (var neighbor in neighbors)
            {
                if (newRegion.Coordinates.Add(neighbor))
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        return newRegion;
    }

    public class Region
    {
        public char RegionId { get; set; }

        public HashSet<Coordinate> Coordinates { get; set; } = [];

        public int GetArea()
        {
            return Coordinates.Count;
        }

        public int GetPerimeter()
        {
            var queue = new Queue<Coordinate>();
            var visited = new HashSet<Coordinate>();
            var firstCoord = Coordinates.First();
            queue.Enqueue(firstCoord);
            visited.Add(firstCoord);

            var neighborCount = 0;
            
            while (queue.Count > 0)
            {
                var next = queue.Dequeue();

                var neighbors = next.GetNeighbors()
                                    .Where(Coordinates.Contains)
                                    .ToList();
                neighborCount += neighbors.Count;
                foreach (var neighbor in neighbors)
                {
                    if (visited.Add(neighbor))
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }

            var totalSides = Coordinates.Count * 4;
            var sharedSides = neighborCount;
            return totalSides - sharedSides;
        }

        public int GetSides(Dictionary<Coordinate, char> map)
        {
            var corners = Coordinates.Select(x =>
                                                      {
                                                          var neighbors = x.GetNeighbors(true);
                                                          var cornerCount = 0;

                                                          if (IsCorner(neighbors, map, Direction.East, Direction.North, Direction.NorthEast))
                                                          {
                                                              cornerCount++;
                                                          }
                                                          
                                                          if (IsCorner(neighbors, map, Direction.East, Direction.South, Direction.SouthEast))
                                                          {
                                                              cornerCount++;
                                                          }
                                                          
                                                          if (IsCorner(neighbors, map, Direction.West, Direction.North, Direction.NorthWest))
                                                          {
                                                              cornerCount++;
                                                          }
                                                          
                                                          if (IsCorner(neighbors, map, Direction.West, Direction.South, Direction.SouthWest))
                                                          {
                                                              cornerCount++;
                                                          }

                                                          return cornerCount;
                                                      })
                                               .Sum();

            return corners;
        }

        private bool IsCorner(List<Coordinate> neighbors, Dictionary<Coordinate, char> map, Direction one, Direction two, Direction combined)
        {
            var oneIsSame = map.TryGetValue(neighbors[(int)one], out var oneChar) && oneChar == this.RegionId;   
            var twoIsSame = map.TryGetValue(neighbors[(int)two], out var twoChar) && twoChar == this.RegionId;   
            
            if (!oneIsSame && !twoIsSame)
            {
                // Convex corner, both directions are different
                return true;
            }
            
            var combinedIsSame = map.TryGetValue(neighbors[(int)combined], out var combinedChar) && combinedChar == this.RegionId;   

            if ((oneIsSame && twoIsSame) && !combinedIsSame)
            {
                // Two directions are same, but combined isn't so concave corner
                return true;
            }

            return false;
        }
    }
}