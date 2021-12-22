using Helpers;
using Helpers.Structure;

var solver = new Solver(new Day21Problem());

await solver.Solve();

class Day21Problem : ProblemBase
{
    private long GetEnabledCubeCount(Cuboid? maxRange)
    {
        var commands = commandInputs.ToList();

        if (maxRange != null)
        {
            // For part 1, alter the input cubes by restricting them to a 100x100x100 cube
            // And remove the ones that don't affect that region at all
            commands = commands.Select(x => new Command
                                            {
                                                On = x.On,
                                                AffectedCube = x.AffectedCube.Intersect(maxRange)
                                            })
                               .Where(x => x.AffectedCube != null)
                               .ToList();
        }

        var reactor = new List<Command>();

        foreach (var command in commands)
        {
            var commandsToAdd = new List<Command>();

            if (command.On)
            {
                // If an on command, add since everything starts off
                commandsToAdd.Add(command);
            }

            foreach (var otherRegion in reactor)
            {
                var intersect = otherRegion.AffectedCube.Intersect(command.AffectedCube);

                if (intersect != null)
                {
                    // If we intersect with existing commands, add an inverse to keep the count accurate
                    // Instead of splitting the cubes (math is hard) we just count both cubes as on and then count the intersect as off
                    commandsToAdd.Add(new Command
                                      {
                                          AffectedCube = intersect,
                                          On = !otherRegion.On
                                      });
                }
            }

            reactor.AddRange(commandsToAdd);
        }

        var onCount = 0L;

        // Now just loop through the processed command list adding and subtracting
        foreach (var resultCommand in reactor)
        {
            if (resultCommand.On)
            {
                onCount += resultCommand.AffectedCube.Size;
            }
            else
            {
                onCount -= resultCommand.AffectedCube.Size;
            }    
        }

        return onCount;
    }

    protected override async Task<string> SolvePartOneInternal()
    {
        var boundingCube = new Cuboid
                           {
                               MinX = -50,
                               MaxX = 50,
                               MinY = -50,
                               MaxY = 50,
                               MinZ = -50,
                               MaxZ = 50,
                           };

        var count = GetEnabledCubeCount(boundingCube);

        return count.ToString();
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        var count = GetEnabledCubeCount(null);

        return count.ToString();
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
    public long MinX { get; set; }

    public long MaxX { get; set; }

    public long MinY { get; set; }

    public long MaxY { get; set; }

    public long MinZ { get; set; }

    public long MaxZ { get; set; }

    public long Size
    {
        get
        {
            return Math.Abs((MaxX - MinX + 1) * (MaxY - MinY + 1) * (MaxZ - MinZ + 1));
        }
    }

    public Cuboid? Intersect(Cuboid other)
    {
        if (this.MinX > other.MaxX
            || this.MaxX < other.MinX
            || this.MinY > other.MaxY
            || this.MaxY < other.MinY
            || this.MinZ > other.MaxZ
            || this.MaxZ < other.MinZ)
        {
            return null;
        }

        var xMin = Math.Max(this.MinX, other.MinX);
        var xMax = Math.Min(this.MaxX, other.MaxX);

        var yMin = Math.Max(this.MinY, other.MinY);
        var yMax = Math.Min(this.MaxY, other.MaxY);

        var zMin = Math.Max(this.MinZ, other.MinZ);
        var zMax = Math.Min(this.MaxZ, other.MaxZ);

        return new Cuboid
               {
                   MinX = xMin,
                   MaxX = xMax,
                   MinY = yMin,
                   MaxY = yMax,
                   MinZ = zMin,
                   MaxZ = zMax
               };
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

        (long min, long max) GetCoordRange(string range)
        {
            var rangeSplit = range.Substring(2)
                                  .Split("..");

            return (long.Parse(rangeSplit[0]), long.Parse(rangeSplit[1]));
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