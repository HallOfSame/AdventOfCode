using Helpers.Structure;

namespace PuzzleDays;

public class Day11 : SingleExecutionPuzzle<Day11.State>
{
    public override PuzzleInfo Info => new(2025, 11, "Reactor");

    protected override async Task<string> ExecutePuzzlePartOne()
    {
        var start = InitialState.Nodes.First(x => x.Label == "you");
        var target = InitialState.Nodes.First(x => x.Label == "out");

        var allPaths = FindPathsToEnd([start], target);

        return allPaths.Count.ToString();
    }

    private List<List<Node>> FindPathsToEnd(List<Node> currentPath, Node target)
    {
        var current = currentPath[^1];

        if (current.Equals(target))
        {
            return [currentPath.ToList()];
        }

        var result = new List<List<Node>>();

        foreach (var connected in current.Connections)
        {
            currentPath.Add(connected);
            result.AddRange(FindPathsToEnd(currentPath, target));
            currentPath.Remove(connected);
        }

        return result;
    }

    protected override async Task<string> ExecutePuzzlePartTwo()
    {
        throw new NotImplementedException();
    }

    protected override async Task<State> LoadInputState(string puzzleInput, PuzzleInputType inputType)
    {
        var nodes = new HashSet<Node>();

        foreach (var line in puzzleInput.Split('\n'))
        {
            var lineSplit = line.Split(':');
            var nodeName = lineSplit[0];

            var connections = lineSplit[1]
                .Trim()
                .Split(' ');

            var node = GetOrCreate(nodeName);

            foreach (var connectionName in connections)
            {
                var connected = GetOrCreate(connectionName);

                node.Connections.Add(connected);
            }
        }

        return new State
        {
            Nodes = nodes.ToList()
        };

        Node GetOrCreate(string name)
        {
            var node = nodes.SingleOrDefault(x => x.Label == name);

            if (node is not null)
            {
                return node;
            }

            node = new Node
            {
                Label = name,
                Connections = []
            };

            nodes.Add(node);

            return node;
        }
    }

    public class State
    {
        public required List<Node> Nodes { get; init; }
    }

    public class Node
    {
        public required string Label { get; init; }
        public required List<Node> Connections { get; init; }

        private bool Equals(Node other)
        {
            return string.Equals(Label, other.Label, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((Node)obj);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Label);
        }
    }
}