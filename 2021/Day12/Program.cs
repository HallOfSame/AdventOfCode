using Helpers.FileReaders;
using Helpers.Structure;

var solver = new Solver(new Day12Problem());

await solver.Solve();

class Day12Problem : ProblemBase
{
    private Map map;

    protected override Task<string> SolvePartOneInternal()
    {
        var startNode = map.Nodes.First(x => x.IsStart);

        var starterPath = new Path
                          {
                              Current = startNode,
                              VisitedNodes = new HashSet<Node>
                                             {
                                                 startNode
                                             },
                              Start = startNode
                          };

        var allPaths = CountPossiblePathsToEnd(starterPath,
                                               false);

        return Task.FromResult(allPaths.ToString());
    }

    protected override Task<string> SolvePartTwoInternal()
    {
        var startNode = map.Nodes.First(x => x.IsStart);

        var starterPath = new Path
                          {
                              Current = startNode,
                              VisitedNodes = new HashSet<Node>
                              {
                                  startNode
                              },
                              Start = startNode
                          };

        var allPaths = CountPossiblePathsToEnd(starterPath,
                                               true);

        return Task.FromResult(allPaths.ToString());
    }

    private int CountPossiblePathsToEnd(Path currentPath,
                                        bool allowDoubleVisitOfOneSmallCave)
    {
        if (currentPath.IsComplete)
        {
            return 1;
        }

        var resultPaths = 0;

        foreach (var neighbor in currentPath.Current.Edges)
        {
            var otherNode = neighbor.GetOther(currentPath.Current);

            var markAsDoubleVisit = false;

            if (otherNode.IsSmall
                && currentPath.VisitedNodes.Contains(otherNode))
            {
                if (!allowDoubleVisitOfOneSmallCave)
                {
                    // Stop processing this pathway, we can't return to a small node
                    continue;
                }

                // Can be allowed in part 2
                if (currentPath.HasVisitedSmallCaveTwice
                    || otherNode.IsStart)
                {
                    continue;
                }

                markAsDoubleVisit = true;
            }

            // Add this node
            var newPath = currentPath.Clone();

            newPath.Current = otherNode;
            newPath.HasVisitedSmallCaveTwice = newPath.HasVisitedSmallCaveTwice || markAsDoubleVisit;

            if (otherNode.IsSmall)
            {
                newPath.VisitedNodes.Add(otherNode);
            }

            resultPaths += CountPossiblePathsToEnd(newPath,
                                                   allowDoubleVisitOfOneSmallCave);
        }

        return resultPaths;
    }

    public override async Task ReadInput()
    {
        var inputEdgeStrings = await new StringFileReader().ReadInputFromFile();

        var nodes = new Dictionary<string, Node>();

        var edges = new List<Edge>();

        foreach (var edgeString in inputEdgeStrings)
        {
            var edgeSplit = edgeString.Split('-');

            var nodeOneId = edgeSplit[0];

            if (!nodes.TryGetValue(nodeOneId,
                                   out var nodeOne))
            {
                nodeOne = new Node(nodeOneId);

                nodes[nodeOneId] = nodeOne;
            }

            var nodeTwoId = edgeSplit[1];

            if (!nodes.TryGetValue(nodeTwoId,
                                   out var nodeTwo))
            {
                nodeTwo = new Node(nodeTwoId);

                nodes[nodeTwoId] = nodeTwo;
            }

            var edge = new Edge
                       {
                           NodeOne = nodeOne,
                           NodeTwo = nodeTwo
                       };

            nodeOne.Edges.Add(edge);
            nodeTwo.Edges.Add(edge);

            edges.Add(edge);
        }

        map = new Map
              {
                  Edges = edges,
                  Nodes = nodes.Values.ToList()
              };
    }
}

class Map
{
    public List<Node> Nodes { get; set; }

    public List<Edge> Edges { get; set; }
}

class Node
{
    public Node(string id)
    {
        Id = id;
        IsSmall = Id.All(char.IsLower);
        IsStart = Id.Equals(StartCaveId);
        IsEnd = Id.Equals(EndCaveId);
    }

    public const string StartCaveId = "start";

    public const string EndCaveId = "end";

    public string Id { get; init; }

    public bool IsSmall { get; }

    public bool IsStart { get; }

    public bool IsEnd { get; }

    public List<Edge> Edges { get; } = new();

    protected bool Equals(Node other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this,
                            obj))
        {
            return true;
        }

        return Equals((Node)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}

class Edge
{ 
    public Node NodeOne { get; init; }
    public Node NodeTwo { get; init; }

    public Node GetOther(Node currentNode)
    {
        return NodeOne.Equals(currentNode) ? NodeTwo : NodeOne;
    }

    public override string ToString()
    {
        return $"{NodeOne.Id}-{NodeTwo.Id}";
    }
}

class Path
{
    public Node Start { get; init; }

    public Node Current { get; set; }

    public bool IsComplete => Start.IsStart && Current.IsEnd;

    public HashSet<Node> VisitedNodes { get; init; } = new();

    public bool HasVisitedSmallCaveTwice { get; set; }

    public Path Clone()
    {
        var clone = new Path
                    {
                        Start = this.Start,
                        Current = this.Current,
                        VisitedNodes = this.VisitedNodes.ToHashSet(),
                        HasVisitedSmallCaveTwice = this.HasVisitedSmallCaveTwice
                    };

        return clone;
    }
}

