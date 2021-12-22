using Helpers;
using Helpers.Maps._3D;
using Helpers.Structure;

var solver = new Solver(new Day21Problem());

await solver.Solve();

class Day21Problem : ProblemBase
{
    protected override async Task<string> SolvePartOneInternal()
    {
        var reactor = new Dictionary<Coordinate3d, bool>();

        for (var x = -50; x <= 50; x++)
        {
            for(var y = -50; y <= 50; y++)
            {
                for (var z = -50; z <= 50; z++)
                {
                    reactor.Add(new Coordinate3d(x,
                                                 y,
                                                 z),
                                false);
                }
            }
        }

        var reactorSize = 100 * 100 * 100;

        foreach (var command in commandInputs)
        {
            if (command.AffectedCube.Size < reactorSize)
            {
                for (var x = command.AffectedCube.MinX; x <= command.AffectedCube.MaxX; x++)
                {
                    for (var y = command.AffectedCube.MinY; y <= command.AffectedCube.MaxY; y++)
                    {
                        for (var z = command.AffectedCube.MinZ; z <= command.AffectedCube.MaxZ; z++)
                        {
                            var coord = new Coordinate3d(x,
                                                         y,
                                                         z);

                            if (reactor.ContainsKey(coord))
                            {
                                reactor[coord] = command.On;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var (coordinate, _) in reactor)
                {
                    if (command.AffectedCube.Contains(coordinate))
                    {
                        reactor[coordinate] = command.On;
                    }
                }
            }
        }

        return reactor.Count(x => x.Value)
                      .ToString();
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        throw new NotImplementedException();
    }

    public override async Task ReadInput()
    {
        commandInputs = await new CommandReader().ReadInputFromFile();
    }

    private List<Command> commandInputs;
}

class Command
{
    public bool On { get; set; }

    public Cuboid AffectedCube { get; set; }
}

class Cuboid
{
    public int MinX { get; set; }

    public int MaxX { get; set; }

    public int MinY { get; set; }

    public int MaxY { get; set; }

    public int MinZ { get; set; }

    public int MaxZ { get; set; }

    public int Size
    {
        get
        {
            return Math.Abs((MaxX - MinX) * (MaxY - MinY) * (MaxZ - MinZ));
        }
    }

    public bool Contains(Coordinate3d coordinate)
    {
        return coordinate.X >= MinX && coordinate.X <= MaxX && coordinate.Y >= MinY && coordinate.Y <= MaxY && coordinate.Z >= MinZ && coordinate.Z <= MaxZ;
    }
}

class CommandReader : FileReader<Command>
{
    protected override Command ProcessLineOfFile(string line)
    {
        var initialSplit = line.Split(' ');

        var turnOn = initialSplit[0] == "on";

        var coordSplit = initialSplit[1]
            .Split(',');

        (int min, int max) GetCoordRange(string range)
        {
            var rangeSplit = range.Substring(2)
                                  .Split("..");

            return (int.Parse(rangeSplit[0]), int.Parse(rangeSplit[1]));
        }

        var xRange = GetCoordRange(coordSplit[0]);

        var yRange = GetCoordRange(coordSplit[1]);

        var zRange = GetCoordRange(coordSplit[2]);

        return new Command
               {
                   On = turnOn,
                   AffectedCube = new Cuboid
                                  {
                                      MinX = xRange.min,
                                      MaxX = xRange.max,
                                      MinY = yRange.min,
                                      MaxY = yRange.max,
                                      MinZ = zRange.min,
                                      MaxZ = zRange.max,
                                  }
               };
    }
}