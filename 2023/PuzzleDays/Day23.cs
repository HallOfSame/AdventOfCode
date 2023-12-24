using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day23 : ProblemBase
    {
        private Func<Coordinate, List<(Coordinate, int)>> getNeighborFunc;

        protected override async Task<string> SolvePartOneInternal()
        {
            endpoint = coordinates.First(x => x.Key.Y == 0 && x.Value == '.').Key;

            var yMax = coordinates.Max(x => x.Key.Y);

            getNeighborFunc = GetNeighborsPart1;

            var result = FindLongestPathDistanceToEnd(coordinates.First(x => x.Key.Y == yMax && x.Value == '.')
                                                          .Key,
                                                      0,
                                                      new HashSet<Coordinate>());

            return result.ToString();
        }

        private Coordinate endpoint;


        private int FindLongestPathDistanceToEnd(Coordinate currentPoint, int currentPathLength, HashSet<Coordinate> visited)
        {
            if (currentPoint == endpoint)
            {
                return currentPathLength;
            }

            visited.Add(currentPoint);

            // Check path length using each valid neighbor
            var pathToEndMax = 0;

            var neighbors = getNeighborFunc(currentPoint);

            var neighborHasEnding = neighbors.Any(x => x.Item1 == endpoint);

            foreach (var (neighbor, cost) in neighbors)
            {
                if (!IsValidStep(neighbor))
                {
                    continue;
                }

                if (visited.Contains(neighbor))
                {
                    continue;
                }

                if (neighborHasEnding && neighbor != endpoint)
                {
                    // If we get to a node that can go to the neighbor there is no path where we don't go to it
                    continue;
                }

                visited.Add(neighbor);

                var pathWithThisStep =
                    FindLongestPathDistanceToEnd(neighbor, currentPathLength + cost, visited);

                visited.Remove(neighbor);

                pathToEndMax = Math.Max(pathToEndMax, pathWithThisStep);
            }

            return pathToEndMax;
        }

        private List<(Coordinate, int)> GetNeighborsPart1(Coordinate c)
        {
            List<(Coordinate, int)> neighbors;

            if (coordinates[c] == '.')
            {
                neighbors = c.GetNeighbors()
                    .Select(x => (x, 1))
                    .ToList();
            }
            else
            {
                var direction = coordinates[c] switch
                {
                    '>' => Direction.East,
                    '^' => Direction.North,
                    '<' => Direction.West,
                    'v' => Direction.South,
                    _ => throw new Exception()
                };

                neighbors = new List<(Coordinate, int)>
                {
                    (c.GetDirection(direction), 1)
                };
            }

            return neighbors;
        }

        private List<(Coordinate, int)> GetNeighborsPart2(Coordinate c)
        {
            return neighborMap[c];
        }

        private Dictionary<Coordinate, List<(Coordinate, int)>> neighborMap = new();

        private List<(Coordinate, int)> FindNeighbors(Coordinate current, HashSet<Coordinate> decisionPoints, HashSet<Coordinate> visited)
        {
            if (decisionPoints.Contains(current))
            {
                return new List<(Coordinate, int)>
                {
                    // -1 because visited contains the node we started at
                    (current, visited.Count - 1)
                };
            }

            var results = new List<(Coordinate, int)>();

            visited.Add(current);

            foreach (var neighbor in current.GetNeighbors().Where(x => IsValidStep(x) && !visited.Contains(x)))
            {
                visited.Add(neighbor);

                results.AddRange(FindNeighbors(neighbor, decisionPoints, visited));

                visited.Remove(neighbor);
            }

            return results;
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var decisionPoints = new HashSet<Coordinate>();

            var yMax = coordinates.Max(x => x.Key.Y);

            var startPoint = coordinates.First(x => x.Key.Y == yMax && x.Value == '.')
                .Key;

            // The map is long hallways connecting certain coordinates where the paths branch
            // First go through and find those decision points
            foreach (var coordinate in coordinates.Keys.Where(IsValidStep))
            {
                var neighbors = coordinate.GetNeighbors()
                    .Count(IsValidStep);

                if (neighbors > 2)
                {
                    decisionPoints.Add(coordinate);
                }
            }

            // Now for each of those, find how they connect to the other decision points and their cost
            foreach (var point in decisionPoints)
            {
                var updatedPoints = decisionPoints.ToHashSet();
                // We don't want to end up at the same point
                updatedPoints.Remove(point);
                // And want to consider the start and end
                updatedPoints.Add(endpoint);
                updatedPoints.Add(startPoint);

                neighborMap[point] = FindNeighbors(point, updatedPoints, new HashSet<Coordinate>())
                    .ToList();
            }

            getNeighborFunc = GetNeighborsPart2;

            var connectionToStartPoint = neighborMap.First(x => x.Value.Any(v => v.Item1 == startPoint));

            neighborMap[startPoint] = new List<(Coordinate, int)>
            {
                (connectionToStartPoint.Key, connectionToStartPoint.Value.Where(x => x.Item1 == startPoint)
                    .Select(x => x.Item2)
                    .First())
            };

            // Now the problem is just part 1 again, but we've significantly compressed the graph to make brute force possible
            var result = FindLongestPathDistanceToEnd(startPoint,
                                                      0,
                                                      new HashSet<Coordinate>());

            return result.ToString();
        }

        private bool IsValidStep(Coordinate coord)
        {
            return coordinates.TryGetValue(coord, out var ch) && ch != '#';
        }

        private Dictionary<Coordinate, char> coordinates;

        public override async Task ReadInput()
        {
            coordinates =
                (await new GridFileReader().ReadInputFromFile()).ToDictionary(x => x.Coordinate, x => x.Value);
        }
    }
}
