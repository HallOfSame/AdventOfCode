using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;

using Priority_Queue;

namespace PuzzleDays
{
    public class Day24 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            blizzardCache[0] = startingBlizzards;

            var queue = new SimplePriorityQueue<(Coordinate expedition, int minute), int>();

            queue.Enqueue((start, 0),
                          0);

            while (queue.Count > 0)
            {
                var (currentLoc, currentMinute) = queue.Dequeue();

                if (currentLoc == end)
                {
                    return currentMinute.ToString();
                }

                var newTime = currentMinute + 1;

                var blizzardsAtNextMove = GetBlizzardsAtMinute(newTime);

                var neighbors = currentLoc.GetNeighbors();

                // Wall check handles everything except trying to move north from the start position
                // That's what the max Y check is for
                var validNeighbors = neighbors.Where(x => !walls.Contains(x) && x.Y <= maxY);

                validNeighbors = validNeighbors.Where(x => !blizzardsAtNextMove.ContainsKey(x));

                int GetNewPriority(Coordinate newPosition)
                {
                    return (newTime * 1000)
                           + CoordinateHelper.ManhattanDistance(newPosition,
                                                                end);
                }

                void AddToQueue(Coordinate newPosition)
                {
                    var prio = GetNewPriority(newPosition);

                    if (!queue.EnqueueWithoutDuplicates((newPosition, newTime),
                                                        prio))
                    {
                        queue.UpdatePriority((newPosition, newTime),
                                             prio);
                    }
                }

                foreach (var neighbor in validNeighbors)
                {
                    AddToQueue(neighbor);
                }

                if (!blizzardsAtNextMove.ContainsKey(currentLoc))
                {
                    AddToQueue(currentLoc);
                }
            }

            throw new Exception("Did not reach goal");
        }

        private Dictionary<int, Dictionary<Coordinate, List<Direction>>> blizzardCache = new();

        private Dictionary<Coordinate, List<Direction>> GetBlizzardsAtMinute(int minute)
        {
            if (blizzardCache.TryGetValue(minute, out var cache))
            {
                return cache;
            }

            var prevMinute = GetBlizzardsAtMinute(minute - 1);

            var newBlizzardMap = new Dictionary<Coordinate, List<Direction>>();

            foreach (var (currentLoc, directions) in prevMinute)
            {
                foreach (var direction in directions)
                {
                    var updatedLocation = direction switch
                    {
                        Direction.North => new Coordinate(currentLoc.X,
                                                          currentLoc.Y + 1),
                        Direction.South => new Coordinate(currentLoc.X,
                                                          currentLoc.Y - 1),
                        Direction.East => new Coordinate(currentLoc.X + 1,
                                                         currentLoc.Y),
                        Direction.West => new Coordinate(currentLoc.X - 1,
                                                         currentLoc.Y),
                    };

                    if (walls.Contains(updatedLocation))
                    {
                        if (updatedLocation.X == 0)
                        {
                            updatedLocation.X = maxX;
                        }

                        if (updatedLocation.X == maxX + 1)
                        {
                            updatedLocation.X = 1;
                        }

                        if (updatedLocation.Y == 0)
                        {
                            updatedLocation.Y = maxY;
                        }

                        if (updatedLocation.Y == maxY + 1)
                        {
                            updatedLocation.Y = 1;
                        }
                    }

                    if (newBlizzardMap.TryGetValue(updatedLocation,
                                                   out var existingList))
                    {
                        existingList.Add(direction);
                    }
                    else
                    {
                        newBlizzardMap[updatedLocation] = new List<Direction>
                                                          {
                                                              direction
                                                          };
                    }
                }
            }

            blizzardCache[minute] = newBlizzardMap;

            return newBlizzardMap;
        }

        private void DrawBlizzardsAtMinute(int minute)
        {
            var blizz = GetBlizzardsAtMinute(minute);

            blizz.Keys.Draw((coord) =>
                            {
                                if (coord == start)
                                {
                                    return ".";
                                }

                                if (coord == end)
                                {
                                    return ".";
                                }

                                if (walls.Contains(coord))
                                {
                                    return "#";
                                }

                                if (blizz.TryGetValue(coord,
                                                      out var dir))
                                {
                                    if (dir.Count > 1)
                                    {
                                        return dir.Count.ToString();
                                    }

                                    return dir.First() switch
                                    {
                                        Direction.North => "^",
                                        Direction.South => "v",
                                        Direction.West => "<",
                                        Direction.East => ">"
                                    };
                                }

                                throw new Exception("Unknown coord");
                            });
        }

        private void DrawMap(int minute, Coordinate currentLocation)
        {
            Console.WriteLine($"At minute: {minute} with location: {currentLocation}");
            var map = walls.ToHashSet();
            map.Add(start);
            map.Add(end);
            map.Add(currentLocation);
            var blizz = GetBlizzardsAtMinute(minute);
            blizz.Keys.ToList()
                 .ForEach(x => map.Add(x));

            map.Draw((coord) =>
                     {
                         if (coord == currentLocation)
                         {
                             return "E";
                         }

                         if (coord == start)
                         {
                             return ".";
                         }

                         if (coord == end)
                         {
                             return ".";
                         }

                         if (walls.Contains(coord))
                         {
                             return "#";
                         }

                         if (blizz.TryGetValue(coord,
                                                   out var dir))
                         {
                             if (dir.Count > 1)
                             {
                                 return dir.Count.ToString();
                             }

                             return dir.First() switch
                             {
                                 Direction.North => "^",
                                 Direction.South => "v",
                                 Direction.West => "<",
                                 Direction.East => ">"
                             };
                         }

                         throw new Exception("Unknown coord");
                     });
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        public override async Task ReadInput()
        {
            var lines = await new StringFileReader().ReadInputFromFile();

            var y = lines.Count - 1;

            foreach (var line in lines)
            {
                for (var x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                    {
                        walls.Add(new Coordinate(x,
                                                 y));
                    }
                    else if (y == lines.Count - 1
                             && line[x] == '.')
                    {
                        start = new Coordinate(x,
                                               y);
                    }
                    else if (y == 0
                             && line[x] == '.')
                    {
                        end = new Coordinate(x,
                                             y);
                    }
                    else if (line[x] != '.')
                    {
                        startingBlizzards[new Coordinate(x,
                                                 y)] = new List<Direction>
                                                       {
                                                           line[x] switch
                                                           {
                                                               '>' => Direction.East,
                                                               '<' => Direction.West,
                                                               '^' => Direction.North,
                                                               'v' => Direction.South
                                                           }
                                                       };
                    }
                }

                y--;
            }

            maxX = walls.Max(x => x.X) - 1;
            maxY = walls.Max(x => x.Y) - 1;
        }

        private Coordinate start;

        private Coordinate end;

        private Dictionary<Coordinate, List<Direction>> startingBlizzards = new();

        private HashSet<Coordinate> walls = new();

        private int maxX;

        private int maxY;
    }
}
