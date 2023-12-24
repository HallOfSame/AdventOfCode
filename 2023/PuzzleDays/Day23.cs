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
        protected override async Task<string> SolvePartOneInternal()
        {
            endpoint = coordinates.First(x => x.Key.Y == 0 && x.Value == '.').Key;

            var yMax = coordinates.Max(x => x.Key.Y);

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

            var neighbors = GetNeighbors(currentPoint);

            foreach (var neighbor in neighbors)
            {
                if (!coordinates.TryGetValue(neighbor, out var ch) || ch == '#')
                {
                    continue;
                }

                if (visited.Contains(neighbor))
                {
                    continue;
                }

                visited.Add(neighbor);

                var pathWithThisStep =
                    FindLongestPathDistanceToEnd(neighbor, currentPathLength + 1, visited);

                visited.Remove(neighbor);

                pathToEndMax = Math.Max(pathToEndMax, pathWithThisStep);
            }

            return pathToEndMax;
        }

        private List<Coordinate> GetNeighbors(Coordinate c)
        {
            List<Coordinate> neighbors;

            if (coordinates[c] == '.')
            {
                neighbors = c.GetNeighbors();
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

                neighbors = new List<Coordinate>
                {
                    c.GetDirection(direction)
                };
            }

            return neighbors;
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        private Dictionary<Coordinate, char> coordinates;

        public override async Task ReadInput()
        {
            coordinates =
                (await new GridFileReader().ReadInputFromFile()).ToDictionary(x => x.Coordinate, x => x.Value);
        }
    }
}
