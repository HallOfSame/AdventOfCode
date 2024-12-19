using Helpers.Drawing;
using Helpers.Extensions;
using Helpers.Heaps;
using Helpers.Interfaces;
using Helpers.Maps;
using Helpers.Structure;
using InputStorageDatabase;

namespace PuzzleDays
{
    public class Day18 : SingleExecutionPuzzle<Day18.ExecState>, IVisualize2d
    {
        public record ExecState(List<Coordinate> FallingBytes, int GridSize, int BytesToDrop);

        public override PuzzleInfo Info => new(2024, 18, "RAM Run");
        protected override async Task<ExecState> LoadInputState(string puzzleInput, PuzzleInputType inputType)
        {
            var lines = puzzleInput.Trim()
                .Split('\n');

            var byteCoordinates = lines.Select(x =>
                {
                    var splitLine = x.Split(',');
                    return new Coordinate(int.Parse(splitLine[0]), int.Parse(splitLine[1]));
                })
                .ToList();

            return new ExecState(byteCoordinates, inputType == PuzzleInputType.Example ? 7 : 71, inputType == PuzzleInputType.Example ? 12 : 1024);
        }

        private Dictionary<Coordinate, char> map = [];
        private HashSet<Coordinate> path = [];

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            map.Clear();
            path.Clear();

            var fallenBytes = InitialState.FallingBytes.Take(InitialState.BytesToDrop);

            map = new Dictionary<Coordinate, char>();

            foreach (var byteToDrop in fallenBytes)
            {
                map[byteToDrop] = '#';
            }

            for (var x = 0; x < InitialState.GridSize; x++)
            {
                for (var y = 0; y < InitialState.GridSize; y++)
                {
                    var coordinate = new Coordinate(x, y);

                    map.TryAdd(coordinate, '.');
                }
            }

            var start = new Coordinate(0, 0);
            var target = new Coordinate(InitialState.GridSize - 1, InitialState.GridSize - 1);
            var distance = new Dictionary<Coordinate, int>();
            var prev = new Dictionary<Coordinate, Coordinate>();
            var priorityQueue = MinHeap.CreateMinHeap<Coordinate>();

            distance[start] = 0;
            priorityQueue.Enqueue(start, 0);

            while (priorityQueue.Count != 0)
            {
                var next = priorityQueue.Dequeue();

                if (next == target)
                {
                    break;
                }

                var neighborsToCheck = next.GetNeighbors()
                    .Where(x => map.TryGetValue(x, out var atNeighbor) && atNeighbor != '#')
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

            var pathBuild = target;

            do
            {
                path.Add(pathBuild);
                pathBuild = prev[pathBuild];
            } while (pathBuild != start);

            path.Add(start);

            return distance[target].ToString();
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            // We handled the first few in part one
            var nextToFallIndex = InitialState.BytesToDrop;

            var currentPath = path.ToHashSet();

            while (true)
            {
                for (; nextToFallIndex < InitialState.FallingBytes.Count; nextToFallIndex++)
                {
                    var nextByte = InitialState.FallingBytes[nextToFallIndex];
                    map[nextByte] = '#';

                    // Drop bytes until one was part of our path
                    if (currentPath.Contains(nextByte))
                    {
                        break;
                    }
                }

                // Then try to find a new path
                currentPath = FindPathToEnd();

                if (currentPath.Count == 0)
                {
                    // Empty return means no path
                    break;
                }
            }

            return InitialState.FallingBytes[nextToFallIndex]
                .ToString();
        }

        private HashSet<Coordinate> FindPathToEnd()
        {
            var start = new Coordinate(0, 0);
            var target = new Coordinate(InitialState.GridSize - 1, InitialState.GridSize - 1);
            var distance = new Dictionary<Coordinate, int>();
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
                    .Where(x => map.TryGetValue(x, out var atNeighbor) && atNeighbor != '#')
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
                return [];
            }

            var pathBuild = target;
            var newPath = new HashSet<Coordinate>();

            do
            {
                newPath.Add(pathBuild);
                pathBuild = prev[pathBuild];
            } while (pathBuild != start);

            newPath.Add(start);

            return newPath;
        }

        public DrawableCoordinate[] GetCoordinates()
        {
            return map.ToDictionary(x => new Coordinate(x.Key.X, InitialState.GridSize - 1 - x.Key.Y),
                                    x => path.Contains(x.Key) ? 'O' : map[x.Key])
                .ToDrawableCoordinates();
        }
    }
}
