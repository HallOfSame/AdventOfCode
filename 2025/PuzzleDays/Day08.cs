using System.Globalization;
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
        var heap = BuildMinDistanceHeap();

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
        var circuits = GetCircuits(connections);

        return circuits.OrderByDescending(x => x.Count)
            .Select(x => x.Count)
            .Take(3)
            .Aggregate(1, (curr, next) => curr * next).ToString();
    }

    private SimplePriorityQueue<(Coordinate3d, Coordinate3d), decimal> BuildMinDistanceHeap()
    {
        var heap = new SimplePriorityQueue<(Coordinate3d, Coordinate3d), decimal>();

        foreach (var pair in InitialState.BoxLocations.Combinations(2))
        {
            var pairTuple = (pair.First(), pair.Last());
            var distance = CoordinateHelper.EuclideanDistance(pairTuple.Item1, pairTuple.Item2);

            heap.Enqueue(pairTuple, distance);
        }

        return heap;
    }

    private List<HashSet<Coordinate3d>> GetCircuits(Dictionary<Coordinate3d, HashSet<Coordinate3d>> connections)
    {
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
        return circuits;
    }

    protected override async Task<string> ExecutePuzzlePartTwo()
    {
        // Determine distances
        var heap = BuildMinDistanceHeap();

        // Start making connections, we know from the puzzle we need to make at least the part 1 amount
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
        var circuits = GetCircuits(connections);

        (Coordinate3d, Coordinate3d) lastMin;

        // Now the real part 2 begins
        // Make more connections until we join two that were not previously in the same circuit
        while (true)
        {
            while (true)
            {
                lastMin = heap.Dequeue();

                connections[lastMin.Item1]
                    .Add(lastMin.Item2);
                connections[lastMin.Item2]
                    .Add(lastMin.Item1);

                var circuitOne = circuits.First(x => x.Contains(lastMin.Item1));

                if (!circuitOne.Contains(lastMin.Item2))
                {
                    break;
                }
            }

            // Re-calculate circuits
            circuits = GetCircuits(connections);

            if (circuits.Count == 1)
            {
                break;
            }
        }

        return (lastMin.Item1.X * lastMin.Item2.X).ToString(CultureInfo.InvariantCulture);
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