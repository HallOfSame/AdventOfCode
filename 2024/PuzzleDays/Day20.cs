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
                    var cheatStart = wall;

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
            throw new NotImplementedException();
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

        public DrawableCoordinate[] GetCoordinates()
        {
            return InitialState.Map.ToDrawableCoordinates();
        }
    }
}
