using System.Diagnostics;

using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day12 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            return DjikstraDistanceToEnd()
                .ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            return DjikstraClosestToEnd()
                .ToString();
        }

        private int DjikstraDistanceToEnd()
        {
            var queue = new PriorityQueue<GridSquare, int>();

            var distanceDict = new Dictionary<GridSquare, int>();

            foreach (var (_, square) in grid)
            {
                if (square == start)
                {
                    queue.Enqueue(square,
                                  0);

                    distanceDict[square] = 0;
                }
                else
                {
                    queue.Enqueue(square,
                                  int.MaxValue);
                    distanceDict[square] = int.MaxValue;
                }
            }

            while (queue.Count > 0)
            {
                queue.TryDequeue(out var currentCoord,
                                 out var distance);

                // Hack to get around not being able to update the queue
                // If it doesn't match the current distance, ignore this item
                if (distanceDict[currentCoord] != distance)
                {
                    continue;
                }

                if (currentCoord.Height == 'E')
                {
                    return distance;
                }

                var viableNeighbors = currentCoord.GetNeighbors()
                                                  .Where(neighbor => grid.ContainsKey(neighbor))
                                                  .Select(neighbor => grid[neighbor])
                                                  .Where(neighborSquare => neighborSquare.HeightInt - currentCoord.HeightInt <= 1)
                                                  .ToList();

                foreach (var neighbor in viableNeighbors)
                {
                    // Find the new distance needed to reach this neighbor
                    var newDistance = distance + 1;

                    // If it's better than our current, update and enqueue it
                    if (distanceDict[neighbor] > newDistance)
                    {
                        distanceDict[neighbor] = newDistance;
                        queue.Enqueue(neighbor,
                                      newDistance);
                    }
                }
            }

            throw new Exception("Never visited end");
        }

        private int DjikstraClosestToEnd()
        {
            var queue = new PriorityQueue<GridSquare, int>();

            var distanceDict = new Dictionary<GridSquare, int>();

            var end = grid.Values.First(x => x.Height == 'E');

            foreach (var (_, square) in grid)
            {
                if (square == end)
                {
                    queue.Enqueue(square,
                                  0);

                    distanceDict[square] = 0;
                }
                else
                {
                    queue.Enqueue(square,
                                  int.MaxValue);
                    distanceDict[square] = int.MaxValue;
                }
            }

            while (queue.Count > 0)
            {
                queue.TryDequeue(out var currentCoord,
                                 out var distance);

                // Hack to get around not being able to update the queue
                // If it doesn't match the current distance, ignore this item
                if (distanceDict[currentCoord] != distance)
                {
                    continue;
                }

                if (currentCoord.Height is 'S' or 'a')
                {
                    return distance;
                }

                var viableNeighbors = currentCoord.GetNeighbors()
                                                  .Where(neighbor => grid.ContainsKey(neighbor))
                                                  .Select(neighbor => grid[neighbor])
                                                  // Sneaky difference from the part 1 alg here
                                                  .Where(neighborSquare => currentCoord.HeightInt - neighborSquare.HeightInt <= 1)
                                                  .ToList();

                foreach (var neighbor in viableNeighbors)
                {
                    // Find the new distance needed to reach this neighbor
                    var newDistance = distance + 1;

                    // If it's better than our current, update and enqueue it
                    if (distanceDict[neighbor] > newDistance)
                    {
                        distanceDict[neighbor] = newDistance;
                        queue.Enqueue(neighbor,
                                      newDistance);
                    }
                }
            }

            throw new Exception("Never found a starting point");
        }

        public override async Task ReadInput()
        {
            var lines = await new StringFileReader().ReadInputFromFile();

            var yCoord = lines.Count - 1;

            var squares = new List<GridSquare[]>();

            foreach (var line in lines)
            {
                var thisRow = line.ToCharArray()
                                  .Select((x,
                                           idx) => new GridSquare
                                                   {
                                                       Height = x,
                                                       X = idx,
                                                       Y = yCoord
                                                   })
                                  .ToArray();

                squares.Add(thisRow);

                yCoord--;
            }

            grid = squares.SelectMany(x => x)
                          .ToDictionary(x => new Coordinate(x.X,
                                                            x.Y),
                                        x => x);
            start = grid.First(x => x.Value.Height == 'S')
                        .Value;
        }

        private Dictionary<Coordinate, GridSquare> grid;

        private GridSquare start;
    }
}

[DebuggerDisplay("{Height} - ({X}, {Y})")]
class GridSquare : Coordinate
{
    private char InternalHeight
    {
        get
        {
            if (Height == 'S')
            {
                return 'a';
            }

            if (Height == 'E')
            {
                return 'z';
            }

            return Height;
        }
    }

    public char Height { get; set; }

    public int HeightInt
    {
        get
        {
            return InternalHeight - 'a';
        }
    }
}