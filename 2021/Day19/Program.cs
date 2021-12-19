using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Maps._3D;
using Helpers.Structure;

using Spectre.Console;

var solver = new Solver(new Day19Problem());

await solver.Solve();

class Day19Problem : ProblemBase
{
    private Dictionary<int, Coordinate3d> offsetMap = new Dictionary<int, Coordinate3d>();

    protected override async Task<string> SolvePartOneInternal()
    {
        const int BeaconsToMatch = 12;

        // First scanner is what all our coordinates will be based on
        var absoluteScanner = Scanners[0];

        var remainingScanners = Scanners.Skip(1)
                                        .ToList();

        while (remainingScanners.Any())
        {
            // Find the next scanner where we can match 12 beacons
            Scanner matchedScanner = null;
            int rotationToMatch;
            Coordinate3d offset;

            foreach (var scanner in remainingScanners)
            {
                var allPossibleRotationsOfBeacons = Enumerable.Range(0,
                                                                     24)
                                                              .Select(rotation => new
                                                                                  {
                                                                                      Rotation = rotation,
                                                                                      UpdatedBeacons = scanner.DetectedBeacons.Select(x => x.Rotate(rotation))
                                                                                                              .ToArray()
                                                                                  })
                                                              .ToArray();

                foreach (var orientation in allPossibleRotationsOfBeacons)
                {
                    var vectorCount = new Dictionary<Coordinate3d, int>();

                    foreach (var beacon in absoluteScanner.DetectedBeacons)
                    {
                        foreach (var orientedBeacon in orientation.UpdatedBeacons)
                        {
                            var vector = beacon - orientedBeacon;

                            if (!vectorCount.ContainsKey(vector))
                            {
                                vectorCount[vector] = 1;
                            }
                            else
                            {
                                vectorCount[vector]++;
                            }
                        }
                    }

                    var matchedVector = vectorCount.Where(x => x.Value >= BeaconsToMatch)
                                                   .ToArray();

                    if (matchedVector.Length == 1)
                    {
                        matchedScanner = scanner;
                        rotationToMatch = orientation.Rotation;
                        offset = matchedVector[0].Key;

                        offsetMap[scanner.ScannerNumber] = offset;

                        AnsiConsole.MarkupLine($"[blue]Adding scanner {matchedScanner} with offset {offset}.[/]");

                        absoluteScanner.DetectedBeacons = absoluteScanner.DetectedBeacons.Concat(orientation.UpdatedBeacons.Select(beacon =>
                                                                                                                                   {
                                                                                                                                       var offsetCoordinate = beacon + offset;

                                                                                                                                       return new Beacon
                                                                                                                                              {
                                                                                                                                                  BeaconId = $"{beacon.BeaconId} - Abs",
                                                                                                                                                  X = offsetCoordinate.X,
                                                                                                                                                  Y = offsetCoordinate.Y,
                                                                                                                                                  Z = offsetCoordinate.Z
                                                                                                                                              };
                                                                                                                                   }))
                                                                         .Distinct()
                                                                         .ToArray();
                        break;
                    }
                }

                if (matchedScanner != null)
                {
                    remainingScanners.Remove(matchedScanner);
                    break;
                }
            }

            if (matchedScanner == null)
            {
                throw new ApplicationException("Did not find matching scanner.");
            }
        }

        return absoluteScanner.DetectedBeacons.Length.ToString();
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        var biggestDistance = 0;

        int ManhattanDistance(Coordinate3d one,
                              Coordinate3d two)
        {
            return Math.Abs(one.X - two.X) + Math.Abs(one.Y - two.Y) + Math.Abs(one.Z - two.Z);
        }

        foreach (var offset in offsetMap)
        {
            foreach (var innerOffset in offsetMap)
            {
                if (offset.Key == innerOffset.Key)
                {
                    continue;
                }

                var distance = ManhattanDistance(offset.Value,
                                                 innerOffset.Value);

                if (distance > biggestDistance)
                {
                    biggestDistance = distance;
                    AnsiConsole.MarkupLine($"[aqua]Found new biggest distance {distance} between {offset.Key} and {innerOffset.Key}.[/]");
                }
            }
        }

        return biggestDistance.ToString();
    }

    public override async Task ReadInput()
    {
        var strings = await new StringFileReader().ReadInputFromFile();

        var scanners = new List<Scanner>();

        var beacons = new List<Beacon>();

        var beaconNum = 0;

        var scannerNum = 0;

        foreach (var s in strings.Skip(1))
        {
            if (string.IsNullOrEmpty(s))
            {
                continue;
            }

            if (s.Contains("scanner"))
            {
                scanners.Add(new Scanner(beacons.ToArray(),
                                         scannerNum++));

                beacons.Clear();
                beaconNum = 0;
            }
            else
            {
                var split = s.Split(',');

                var newBeacon = new Beacon
                                {
                                    X = int.Parse(split[0]),
                                    Y = int.Parse(split[1]),
                                    Z = int.Parse(split[2]),
                                    BeaconId = $"{scannerNum} - {beaconNum++}"
                                };

                beacons.Add(newBeacon);
            }
        }

        // Final scanner
        scanners.Add(new Scanner(beacons.ToArray(),
                                 scannerNum));

        this.Scanners = scanners.ToArray();
    }

    private Scanner[] Scanners { get; set; }
}

class Scanner
{
    public int ScannerNumber { get; init; }

    public Scanner(Beacon[] detectedBeacons,
                   int scannerNumber)
    {
        DetectedBeacons = detectedBeacons;
        ScannerNumber = scannerNumber;
    }

    public Beacon[] DetectedBeacons { get; set; }

    public override string ToString()
    {
        return $"Scanner: {ScannerNumber}";
    }
}

class Beacon : Coordinate3d
{
    public string BeaconId { get; set; }

    public override string ToString()
    {
        return $"{BeaconId} - {base.ToString()}";
    }

    public Beacon Rotate(int rotationIndex)
    {
        var updatedX = this.X;
        var updatedY = this.Y;
        var updatedZ = this.Z;

        // Math is too hard so enjoy some hard coded rotations
        switch (rotationIndex)
        {
            // 0-3 X == X
            case 0:
                break;
            case 1:
                updatedY = -this.Z;
                updatedZ = this.Y;
                break;
            case 2:
                updatedY = -this.Y;
                updatedZ = -this.Z;
                break;
            case 3:
                updatedY = this.Z;
                updatedZ = -this.Y;
                break;
            // 4-7 X == -X
            case 4:
                updatedX = -this.X;
                updatedY = -this.Y;
                break;
            case 5:
                updatedX = -this.X;
                updatedY = this.Z;
                updatedZ = this.Y;
                break;
            case 6:
                updatedX = -this.X;
                updatedZ = -this.Z;
                break;
            case 7:
                updatedX = -this.X;
                updatedY = -this.Z;
                updatedZ = -this.Y;
                break;
            // 8-11 X == Y
            case 8:
                updatedX = this.Y;
                updatedY = this.Z;
                updatedZ = this.X;
                break;
            case 9:
                updatedX = this.Y;
                updatedY = -this.X;
                break;
            case 10:
                updatedX = this.Y;
                updatedY = -this.Z;
                updatedZ = -this.X;
                break;
            case 11:
                updatedX = this.Y;
                updatedY = this.X;
                updatedZ = -this.Z;
                break;
            // 12-15 X == -Y
            case 12:
                updatedX = -this.Y;
                updatedY = -this.Z;
                updatedZ = this.X;
                break;
            case 13:
                updatedX = -this.Y;
                updatedY = this.X;
                break;
            case 14:
                updatedX = -this.Y;
                updatedY = this.Z;
                updatedZ = -this.X;
                break;
            case 15:
                updatedX = -this.Y;
                updatedY = -this.X;
                updatedZ = -this.Z;
                break;
            // 16-19 X == Z
            case 16:
                updatedX = this.Z;
                updatedY = this.X;
                updatedZ = this.Y;
                break;
            case 17:
                updatedX = this.Z;
                updatedY = -this.Y;
                updatedZ = this.X;
                break;
            case 18:
                updatedX = this.Z;
                updatedY = -this.X;
                updatedZ = -this.Y;
                break;
            case 19:
                updatedX = this.Z;
                updatedY = this.Y;
                updatedZ = -this.X;
                break;
            // 20-23 X == -Z
            case 20:
                updatedX = -this.Z;
                updatedY = -this.X;
                updatedZ = this.Y;
                break;
            case 21:
                updatedX = -this.Z;
                updatedY = this.Y;
                updatedZ = this.X;
                break;
            case 22:
                updatedX = -this.Z;
                updatedY = this.X;
                updatedZ = -this.Y;
                break;
            case 23:
                updatedX = -this.Z;
                updatedY = -this.Y;
                updatedZ = -this.X;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(rotationIndex), $"Value {rotationIndex} is outside of range.");
        }

        return new Beacon
               {
                   BeaconId = this.BeaconId + $" Rot: {rotationIndex}",
                   X = updatedX,
                   Y = updatedY,
                   Z = updatedZ
               };
    }
}