using Helpers.Drawing;
using Helpers.FileReaders;
using Helpers.Heaps;
using Helpers.Interfaces;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day16 : SingleExecutionPuzzle<Day16.ExecState>, IVisualize2d
    {
        public record ExecState(Dictionary<Coordinate, char> Map);

        public override PuzzleInfo Info => new(2024, 16, "Reindeer Maze");

        protected override async Task<ExecState> LoadInputState(string puzzleInput)
        {
            var grid = await new GridFileReader().ReadFromString(puzzleInput.Trim());
            return new ExecState(grid.ToDictionary(x => x.Coordinate, x => x.Value));
        }

        protected override async Task<string> ExecutePuzzlePartOne()
        {
            var heapToCheck = MinHeap.CreateMinHeap<(Coordinate location, Direction facing, int score)>();
            var start = InitialState.Map.Single(x => x.Value == 'S');
            var end = InitialState.Map.Single(x => x.Value == 'E');
            var visited = new HashSet<(Coordinate, Direction)>();

            heapToCheck.Enqueue((start.Key, Direction.East, 0), 0);

            while (heapToCheck.Count != 0)
            {
                var next = heapToCheck.Dequeue();

                if (next.location == end.Key)
                {
                    return next.score.ToString();
                }

                var forward = next.location.GetDirection(next.facing);
                var atForwardLocation = InitialState.Map[forward];

                if (atForwardLocation != '#')
                {
                    EnqueueLocation(forward, next.facing, next.score + 1);
                }

                var turnRight = next.facing.TurnRight90();
                EnqueueLocation(next.location, turnRight, next.score + 1000);
                var turnLeft = next.facing.TurnLeft90();
                EnqueueLocation(next.location, turnLeft, next.score + 1000);
            }

            throw new InvalidOperationException("Did not find end of maze");

            void EnqueueLocation(Coordinate location, Direction facing, int newScore)
            {
                if (visited.Add((location, facing)))
                {
                    heapToCheck.Enqueue((location, facing, newScore), newScore);
                }
            }
        }

        protected override async Task<string> ExecutePuzzlePartTwo()
        {
            bestPathCoordinates = [];
            var x = DjikstraSearchAllBestPaths();
            return x.ToString();
        }

        private int DjikstraSearchAllBestPaths()
        {
            // Djikstra search
            // Since we can turn, and it affects the distance, the nodes of the graph are location + direction currently faced
            // So that we can model for example that facing north on 1,1 is a 1 to move to 1, 2 but facing east it is 1001, and south would be 2001
            var distance = new Dictionary<(Coordinate location, Direction facing), int>();
            var prev = new Dictionary<(Coordinate location, Direction facing), HashSet<(Coordinate location, Direction facing)>>();
            var priorityQueue = MinHeap.CreateMinHeap<(Coordinate location, Direction facing)>();
            var start = InitialState.Map.Single(x => x.Value == 'S')
                .Key;

            distance[(start, Direction.East)] = 0;
            priorityQueue.Enqueue((start, Direction.East), 0);

            while (priorityQueue.Count != 0)
            {
                // Get next node to check
                var next = priorityQueue.Dequeue();

                // Get all neighbors
                (Coordinate location, Direction facing, int cost)[] neighbors =
                [
                    (next.location.GetDirection(next.facing), next.facing, 1),
                    (next.location, next.facing.TurnLeft90(), 1000),
                    (next.location, next.facing.TurnRight90(), 1000),
                ];

                // Skipping straight neighbor if it is a wall
                var skip = InitialState.Map[neighbors[0].location] == '#' ? 1 : 0;

                foreach (var neighbor in neighbors.Skip(skip))
                {
                    // Calc the distance it took us to get here
                    var calculatedDistance = distance[next] + neighbor.cost;

                    // If it isn't better, keep going
                    var existingDistance = distance.GetValueOrDefault((neighbor.location, neighbor.facing), int.MaxValue);
                    if (calculatedDistance > existingDistance)
                    {
                        continue;
                    }

                    if (calculatedDistance == existingDistance)
                    {
                        // We found another valid way here
                        if (!prev.TryGetValue((neighbor.location, neighbor.facing), out var existingSet))
                        {
                            existingSet = [];
                        }

                        existingSet.Add(next);
                        continue;
                    }

                    // This way is guaranteed better than any we found before
                    // Update our trackers and enqueue
                    prev[(neighbor.location, neighbor.facing)] = [next];
                    distance[(neighbor.location, neighbor.facing)] = calculatedDistance;
                    priorityQueue.Enqueue((neighbor.location, neighbor.facing), calculatedDistance);
                }
            }

            var end = InitialState.Map.Single(x => x.Value == 'E')
                .Key;
            // Find best distance to the end
            var bestDistance = distance.Where(x => x.Key.location == end)
                .Min(x => x.Value);
            // Then get any location + direction that can meet that
            var validEndStates = distance.Where(x => x.Key.location == end && x.Value == bestDistance)
                .Select(x => x.Key)
                .ToList();
            
            // Now we just need to recreate the list of paths
            bestPathCoordinates.Add(end);
            var queue = new Queue<(Coordinate location, Direction facing)>();
            validEndStates.ForEach(queue.Enqueue);

            while (queue.Count != 0)
            {
                var next = queue.Dequeue();

                if (InitialState.Map[next.location] == 'S')
                {
                    continue;
                }

                var backtrackStates = prev[next];

                foreach (var state in backtrackStates)
                {
                    bestPathCoordinates.Add(state.location); 
                    queue.Enqueue(state);
                }
            }

            return bestPathCoordinates.Count;
        }

        private HashSet<Coordinate> bestPathCoordinates = [];

        public DrawableCoordinate[] GetCoordinates()
        {
            return InitialState.Map.ToDictionary(x => x.Key, x => bestPathCoordinates.Contains(x.Key) ? 'O' : x.Value)
                .ToDrawableCoordinates(x => bestPathCoordinates.Contains(x) ? "green" : null);
        }
    }
}
