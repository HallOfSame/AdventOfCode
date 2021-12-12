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

        var allPaths = FindPathToEnd(starterPath);

        return Task.FromResult(allPaths.Count.ToString());
    }

    private List<Path> FindPathToEnd(Path currentPath)
    {
        if (currentPath.IsComplete)
        {
            return new List<Path>
                   {
                       currentPath
                   };
        }

        var resultPaths = new List<Path>();

        foreach (var neighbor in currentPath.Current.Edges)
        {
            var otherNode = neighbor.GetOther(currentPath.Current);

            if (currentPath.VisitedNodes.Contains(otherNode)
                && otherNode.IsSmall)
            {
                // Stop processing this pathway, we can't return to a small node
                continue;
            }

            // Add this node
            var newPath = currentPath.Clone();

            newPath.Current = otherNode;
            newPath.VisitedNodes.Add(otherNode);
            newPath.Edges.Add(neighbor);

            resultPaths.AddRange(FindPathToEnd(newPath));
        }

        return resultPaths;
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        throw new NotImplementedException();
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
                nodeOne = new Node
                          {
                              Id = nodeOneId
                          };

                nodes[nodeOneId] = nodeOne;
            }

            var nodeTwoId = edgeSplit[1];

            if (!nodes.TryGetValue(nodeTwoId,
                                   out var nodeTwo))
            {
                nodeTwo = new Node
                          {
                              Id = nodeTwoId
                          };

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
    public const string StartCaveId = "start";

    public const string EndCaveId = "end";

    public string Id
    {
        get; init;
    }

    public bool IsSmall
    {
        get
        {
            return Id.All(char.IsLower);
        }
    }

    public bool IsStart => Id.Equals(StartCaveId);

    public bool IsEnd => Id.Equals(EndCaveId);

    public List<Edge> Edges { get; } = new List<Edge>();

    protected bool Equals(Node other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null,
                            obj))
        {
            return false;
        }

        if (ReferenceEquals(this,
                            obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
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
}

class Path
{
    public Node Start { get; init; }

    public Node Current { get; set; }

    public bool IsComplete => Start.IsStart && Current.IsEnd;

    public HashSet<Node> VisitedNodes { get; init; } = new();

    public List<Edge> Edges { get; init; } = new();

    public Path Clone()
    {
        return new Path
               {
                   Start = this.Start,
                   Current = this.Current,
                   VisitedNodes = VisitedNodes.ToHashSet(),
                   Edges = Edges.ToList()
               };
    }
}

