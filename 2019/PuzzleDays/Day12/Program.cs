using Helpers;

var moons = await new MoonFileReader().ReadInputFromFile();

var moonPairs = moons.SelectMany((_,
                                  idx) => moons.Skip(idx + 1),
                                 (moonOne,
                                  moonTwo) => (moonOne, moonTwo))
                     .ToList();

var targetStep = 10;

var repeatedXStep = default(int?);
var repeatedYStep = default(int?);
var repeatedZStep = default(int?);

var totalEnergy = 0;

var currentStep = 0;

// Using a string to easily track the state
// Since X Y and Z are independent, we can check how often a single one would repeat first
// This is a lot lower amount of steps to process
var xPostions = new HashSet<string>
                {
                    string.Join(string.Empty,
                                moons.Select(x => x.CurrentPosition.X).Concat(moons.Select(x => x.CurrentVelocity.X)))
                };

var yPostions = new HashSet<string>
                {
                    string.Join(string.Empty,
                                moons.Select(x => x.CurrentPosition.Y).Concat(moons.Select(x => x.CurrentVelocity.Y)))
                };
var zPostions = new HashSet<string>
                {
                    string.Join(string.Empty,
                                moons.Select(x => x.CurrentPosition.Z).Concat(moons.Select(x => x.CurrentVelocity.Z)))
                };

while (true)
{
    currentStep++;

    foreach (var (moonOne, moonTwo) in moonPairs)
    {
        Moon.ApplyGravity(moonOne, moonTwo);
    }

    moons.ForEach(x => x.ApplyVelocity());

    if (currentStep == targetStep)
    {
        // Assuming this will take less time than part 2
        totalEnergy = moons.Select(x => x.GetTotalEnergy())
                           .Sum();
    }

    if (!repeatedXStep.HasValue)
    {
        var currentXPosition = string.Join(string.Empty,
                                           moons.Select(x => x.CurrentPosition.X)
                                                .Concat(moons.Select(x => x.CurrentVelocity.X)));

        if (xPostions.Contains(currentXPosition))
        {
            repeatedXStep = currentStep;
        }
        else
        {
            xPostions.Add(currentXPosition);
        }
    }

    if (!repeatedYStep.HasValue)
    {
        var currentYPosition = string.Join(string.Empty,
                                           moons.Select(x => x.CurrentPosition.Y)
                                                .Concat(moons.Select(x => x.CurrentVelocity.Y)));

        if (yPostions.Contains(currentYPosition))
        {
            repeatedYStep = currentStep;
        }
        else
        {
            yPostions.Add(currentYPosition);
        }
    }

    if (!repeatedZStep.HasValue)
    {
        var currentZPosition = string.Join(string.Empty,
                                           moons.Select(x => x.CurrentPosition.Z)
                                                .Concat(moons.Select(x => x.CurrentVelocity.Z)));

        if (zPostions.Contains(currentZPosition))
        {
            repeatedZStep = currentStep;
        }
        else
        {
            zPostions.Add(currentZPosition);
        }
    }

    if (repeatedXStep.HasValue
        && repeatedYStep.HasValue
        && repeatedZStep.HasValue)
    {
        break;
    }
}

Console.WriteLine($"Total energy after {targetStep} steps: {totalEnergy}.");

// Then just find the first number that the three repeats would coincide
Console.WriteLine($"Repeated position step: {LCM(repeatedXStep.Value, LCM(repeatedYStep.Value, repeatedZStep.Value))}");

// Thanks S/O for these methods for like the 10th time over all the AOC puzzles I've done
static long GCF(long a, long b)
{
    while (b != 0)
    {
        var temp = b;
        b = a % b;
        a = temp;
    }
    return a;
}

static long LCM(long a, long b)
{
    return (a / GCF(a, b)) * b;
}

class MoonFileReader : FileReader<Moon>
{
    protected override Moon ProcessLineOfFile(string line)
    {
        var coordinates = line.Substring(1,
                                         line.Length - 2)
                              .Split(", ")
                              .Select(x => int.Parse(x.Substring(2)))
                              .ToArray();

        return new Moon(new Coordinate3d
                        {
                            X = coordinates[0],
                            Y = coordinates[1],
                            Z = coordinates[2]
                        },
                        new Velocity3d());
    }
}

class Moon
{
    public Moon(Coordinate3d pos,
                Velocity3d vel)
    {
        CurrentPosition = pos;
        CurrentVelocity = vel;
    }

    public Coordinate3d CurrentPosition { get; }

    public Velocity3d CurrentVelocity { get; }

    public void ApplyVelocity()
    {
        CurrentPosition.ApplyVelocity(CurrentVelocity);
    }

    public int GetTotalEnergy()
    {
        return CurrentPosition.AbsoluteSum * CurrentVelocity.AbsoluteSum;
    }

    public static void ApplyGravity(Moon moonOne,
                                    Moon moonTwo)
    {
        UpdateSingleVelocity(() => moonOne.CurrentPosition.X,
                             () => moonTwo.CurrentPosition.X,
                             x => moonOne.CurrentVelocity.X += x,
                             x => moonTwo.CurrentVelocity.X += x);

        UpdateSingleVelocity(() => moonOne.CurrentPosition.Y,
                             () => moonTwo.CurrentPosition.Y,
                             x => moonOne.CurrentVelocity.Y += x,
                             x => moonTwo.CurrentVelocity.Y += x);

        UpdateSingleVelocity(() => moonOne.CurrentPosition.Z,
                             () => moonTwo.CurrentPosition.Z,
                             x => moonOne.CurrentVelocity.Z += x,
                             x => moonTwo.CurrentVelocity.Z += x);
    }

    public static void UpdateSingleVelocity(Func<int> getMoonOnePos,
                                            Func<int> getMoonTwoPos,
                                            Action<int> updateMoonOne,
                                            Action<int> updateMoonTwo)
    {
        var moonTwoAdjustment = getMoonOnePos().CompareTo(getMoonTwoPos());

        var moonOneAdjustment = moonTwoAdjustment * -1;

        updateMoonOne(moonOneAdjustment);
        updateMoonTwo(moonTwoAdjustment);
    }
}

class Coordinate3d
{
    public int X { get; set; }

    public int Y { get; set; }

    public int Z { get; set; }

    public int AbsoluteSum
    {
        get
        {
            return Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
        }
    }

    public void ApplyVelocity(Velocity3d velocity)
    {
        X += velocity.X;
        Y += velocity.Y;
        Z += velocity.Z;
    }
}

class Velocity3d
{
    public int X { get; set; }

    public int Y { get; set; }

    public int Z { get; set; }

    public int AbsoluteSum
    {
        get
        {
            return Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
        }
    }
}