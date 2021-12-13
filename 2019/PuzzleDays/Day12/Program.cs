using Helpers;

var moons = await new MoonFileReader().ReadInputFromFile();

var moonPairs = moons.SelectMany((_,
                                  idx) => moons.Skip(idx + 1),
                                 (moonOne,
                                  moonTwo) => (moonOne, moonTwo))
                     .ToList();

var numberOfSteps = 1000;

for (var currentStep = 1; currentStep <= numberOfSteps; currentStep++)
{
    foreach (var pair in moonPairs)
    {
        Moon.ApplyGravity(pair.moonOne, pair.moonTwo);
    }

    moons.ForEach(x => x.ApplyVelocity());
}

var totalEnergy = moons.Select(x => x.GetTotalEnergy())
                       .Sum();

Console.WriteLine($"Total energy after {numberOfSteps} steps: {totalEnergy}.");

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