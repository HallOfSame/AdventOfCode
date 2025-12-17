using Helpers.Extensions;
using Helpers.Heaps;
using Helpers.Maps;
using Helpers.Maps._3D;
using Helpers.Structure;
using Priority_Queue;

namespace PuzzleDays;

public class Day08 : SingleExecutionPuzzle<Day08.State>
{
    public override PuzzleInfo Info => new(2025, 8, "Playground");

    protected override async Task<string> ExecutePuzzlePartOne()
    {
        // Determine distances
        var heap = new SimplePriorityQueue<(Coordinate3d, Coordinate3d), decimal>();

        foreach (var pair in InitialState.BoxLocations.Combinations(2))
        {
            var pairTuple = (pair.First(), pair.Last());
            var distance = CoordinateHelper.EuclideanDistance(pairTuple.Item1, pairTuple.Item2);

            heap.Enqueue(pairTuple, distance);
        }

        // Start making connections
        var connections = InitialState.BoxLocations.ToDictionary(x => x, _ => new HashSet<Coordinate3d>());

        for (var i = 0; i < InitialState.PartOneConnections; i++)
        {
            var nextMin = heap.Dequeue();

            connections[nextMin.Item1]
                .Add(nextMin.Item2);
            connections[nextMin.Item2]
                .Add(nextMin.Item1);
        }

        // Count circuits
        var visited = new HashSet<Coordinate3d>();

        var first = InitialState.BoxLocations.First();
        var queue = new Queue<Coordinate3d>();
        queue.Enqueue(first);
        var circuits = new List<HashSet<Coordinate3d>>();
        HashSet<Coordinate3d> currentCircuit = [first];

        while (visited.Count != InitialState.BoxLocations.Count)
        {
            if (!queue.TryDequeue(out var toCheck))
            {
                circuits.Add(currentCircuit);
                currentCircuit = [];
                queue.Enqueue(InitialState.BoxLocations.First(x => !visited.Contains(x)));
                continue;
            }

            visited.Add(toCheck);
            currentCircuit.Add(toCheck);

            var connected = connections[toCheck];

            foreach (var neededToCheck in connected.Where(x => !visited.Contains(x)))
            {
                queue.Enqueue(neededToCheck);
            }
        }

        circuits.Add(currentCircuit);

        return circuits.OrderByDescending(x => x.Count)
            .Select(x => x.Count)
            .Take(3)
            .Aggregate(1, (curr, next) => curr * next).ToString();
    }

    protected override async Task<string> ExecutePuzzlePartTwo()
    {
        throw new NotImplementedException();
    }

    protected override async Task<State> LoadInputState(string puzzleInput, PuzzleInputType inputType)
    {
        var locations = new HashSet<Coordinate3d>();

        foreach (var line in puzzleInput.Split('\n'))
        {
            var split = line.Split(',');
            locations.Add(new Coordinate3d(decimal.Parse(split[0]),
                                           decimal.Parse(split[1]),
                                           decimal.Parse(split[2])));
        }

        return new State
        {
            BoxLocations = locations,
            PartOneConnections = inputType == PuzzleInputType.Example ? 10 : 1000
        };
    }

    public class State
    {
        public required HashSet<Coordinate3d> BoxLocations { get; set; }
        public int PartOneConnections { get; set; }
    }
}