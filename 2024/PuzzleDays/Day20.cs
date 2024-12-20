using Helpers.Drawing;
using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Heaps;
using Helpers.Interfaces;
using Helpers.Maps;
using Helpers.Structure;
using InputStorageDatabase;

namespace PuzzleDays
{
    public class Day20 : SingleExecutionPuzzle<Day20.ExecState>, IVisualize2d
    {
        public record ExecState(Dictionary<Coordinate, char> Map);

        public override PuzzleInfo Info => new(2024, 20, "Race Condition");
        protected override async Task<ExecState> LoadInputState(string puzzleInput, PuzzleInputType inputType)
        {
            var grid = await new GridFileReader().ReadFromString(puzzleInput.Trim());

            return new ExecState(grid.ToDictionary(x => x.Coordinate, x => x.Value));
        }

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var end = InitialState.Map.First(x => x.Value == 'E')
                .Key;
            var start = InitialState.Map.First(x => x.Value == 'S')
                .Key;
            var (distanceToEnd, fastestPathNoCheat) = FindPath(end, start);
            var fastestTimeNoCheat = distanceToEnd[start];
            var result = 0;
            
            var j = new Dictionary<int, int>();

            // -2 because there is no point checking starting at the end
            for (var i = 0; i < fastestPathNoCheat.Count - 2; i++)
            {
                var stepOnPath = fastestPathNoCheat[i];

                var wallsToCheatInto = stepOnPath.GetNeighbors()
                    .Where(x => InitialState.Map.IsCharAtCoordinate(x, '#'))
                    .ToList();

                foreach (var wall in wallsToCheatInto)
                {
                    var endPoints = wall.GetNeighbors()
                        .Where(x => x != stepOnPath && (InitialState.Map.IsCharAtCoordinate(x, '.') || x == end))
                        .ToList();

                    foreach (var cheatEnd in endPoints)
                    {
                        int savings;

                        if (cheatEnd == end)
                        {
                            var pathLengthWithCheat = i + 2;
                            savings = fastestTimeNoCheat - pathLengthWithCheat;
                        }
                        else
                        {
                            var pathAfterCheat = distanceToEnd[cheatEnd];

                            if (pathAfterCheat == int.MaxValue)
                            {
                                // Did not come up in the example or real input
                                throw new InvalidOperationException("We haven't discovered this path yet");
                            }

                            // +2 to include stepping into and out of the wall
                            var pathLengthWithCheat = pathAfterCheat + 2 + i;
                            savings = fastestTimeNoCheat - pathLengthWithCheat;
                        }

                        if (savings <= 0)
                        {
                            // Could probably find a way to stop pathfinding this early
                            continue;
                        }

                        j.TryGetValue(savings, out var count);
                        j[savings] = count + 1;

                        if (savings >= 100)
                        {
                            result++;
                        }
                    }
                }
            }

            return result.ToString();
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            var end = InitialState.Map.First(x => x.Value == 'E')
                .Key;
            var start = InitialState.Map.First(x => x.Value == 'S')
                .Key;
            var (distanceToEnd, fastestPathNoCheat) = FindPath(end, start);
            var fastestTimeNoCheat = distanceToEnd[start];
            var result = 0;
            
            var j = new Dictionary<int, int>();
            var checkedCheats = new HashSet<(Coordinate start, Coordinate end)>();

            // -2 because there is no point checking starting at the end
            for (var i = 0; i < fastestPathNoCheat.Count - 2; i++)
            {
                var stepOnPath = fastestPathNoCheat[i];
                var cheatStart = stepOnPath;

                // You can activate a cheat and keep moving on a path apparently
                var endPoints = FindEndpointsForCheat(cheatStart, 20);

                foreach (var (cheatEnd, addedSteps) in endPoints)
                {
                    if (checkedCheats.Contains((cheatStart, cheatEnd)))
                    {
                        continue;
                    }

                    var pathAfterCheat = distanceToEnd[cheatEnd];

                    if (pathAfterCheat == int.MaxValue)
                    {
                        // Did not come up in the example or real input
                        throw new InvalidOperationException("We haven't discovered this path yet");
                    }

                    var pathLengthWithCheat = pathAfterCheat + addedSteps + i;
                    var savings = fastestTimeNoCheat - pathLengthWithCheat;

                    if (savings <= 0)
                    {
                        // Could probably find a way to stop pathfinding this early
                        continue;
                    }

                    j.TryGetValue(savings, out var count);
                    j[savings] = count + 1;

                    if (savings >= 100)
                    {
                        result++;
                    }

                    checkedCheats.Add((cheatStart, cheatEnd));
                }
            }

            return result.ToString();
        }

        private (Dictionary<Coordinate, int>, List<Coordinate>) FindPath(Coordinate start, Coordinate target)
        {
            var distance = InitialState.Map.ToDictionary(x => x.Key, _ => int.MaxValue);
            var prev = new Dictionary<Coordinate, Coordinate>();
            var priorityQueue = MinHeap.CreateMinHeap<Coordinate>();

            distance[start] = 0;
            priorityQueue.Enqueue(start, 0);
            var foundPath = false;

            while (priorityQueue.Count != 0)
            {
                var next = priorityQueue.Dequeue();

                if (next == target)
                {
                    foundPath = true;
                    break;
                }

                var neighborsToCheck = next.GetNeighbors()
                    .Where(x => InitialState.Map.IsCharAtCoordinate(x, '.') || x == target)
                    .ToArray();

                foreach (var neighbor in neighborsToCheck)
                {
                    var calculatedDistance = distance[next] + 1;

                    var existingDistance = distance.GetValueOrDefault(neighbor, int.MaxValue);
                    if (calculatedDistance >= existingDistance)
                    {
                        continue;
                    }

                    prev[neighbor] = next;
                    distance[neighbor] = calculatedDistance;
                    priorityQueue.Enqueue(neighbor, calculatedDistance);
                }
            }

            if (!foundPath)
            {
                throw new InvalidOperationException("Could not find path");
            }

            var pathBuild = target;
            var newPath = new List<Coordinate>();

            do
            {
                newPath.Add(pathBuild);
                pathBuild = prev[pathBuild];
            } while (pathBuild != start);

            newPath.Add(start);

            return (distance, newPath);
        }

        private HashSet<(Coordinate end, int addedSteps)> FindEndpointsForCheat(Coordinate currentPosition, int movesLeft)
        {
            var visited = new HashSet<Coordinate>()
            {
                currentPosition
            };
            var queue = new Queue<(Coordinate position, int movesLeft)>();
            queue.Enqueue((currentPosition, movesLeft));
            var validEndpoints = new HashSet<(Coordinate end, int addedSteps)>();

            while(queue.Count > 0)
            {
                var (next, movesRemaining) = queue.Dequeue();

                if (InitialState.Map[next] == '.' || InitialState.Map[next] == 'E')
                {
                    validEndpoints.Add((next, movesLeft - movesRemaining));
                }

                if (movesRemaining == 0)
                {
                    continue;
                }

                var neighbors = next.GetNeighbors().Where(x => !visited.Contains(x) && InitialState.Map.ContainsKey(x)).ToList();

                foreach(var neighbor in neighbors)
                {
                    visited.Add(neighbor);
                    queue.Enqueue((neighbor, movesRemaining - 1));
                }
            }

            return validEndpoints;
        }

        public DrawableCoordinate[] GetCoordinates()
        {
            return InitialState.Map.ToDrawableCoordinates();
        }
    }
}
