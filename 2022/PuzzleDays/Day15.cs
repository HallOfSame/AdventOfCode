using System.Text.RegularExpressions;

using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day15 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var targetRow = 2_000_000;

            var minX = sensors.Min(x => x.X - x.DistanceToClosestBeacon);

            var maxX = sensors.Max(x => x.X + x.DistanceToClosestBeacon);

            Console.WriteLine($"Searching from {minX} to {maxX}");

            var impossibleBeaconLocations = 0;

            for (var i = minX; i <= maxX; i++)
            {
                var testCoordinate = new Coordinate(i,
                                                    targetRow);

                if (beacons.Contains(testCoordinate))
                {
                    continue;
                }

                var couldNotHaveBeacon = sensors.Any(x => CoordinateHelper.ManhattanDistance(x,
                                                                                             testCoordinate)
                                                          <= x.DistanceToClosestBeacon);

                if (couldNotHaveBeacon)
                {
                    impossibleBeaconLocations++;
                }
            }

            return impossibleBeaconLocations.ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        public override async Task ReadInput()
        {
            var lines = await new StringFileReader().ReadInputFromFile();

            beacons = new HashSet<Coordinate>();
            sensors = new HashSet<Sensor>();

            var parseRegex = new Regex("Sensor at x=(.+), y=(.+): closest beacon is at x=(.+), y=(.+)");

            foreach (var line in lines)
            {
                var regexMatch = parseRegex.Match(line);

                var sensorCoord = new Coordinate(int.Parse(regexMatch.Groups[1]
                                                                     .Value),
                                                 int.Parse(regexMatch.Groups[2]
                                                                     .Value));

                var beaconCoord = new Coordinate(int.Parse(regexMatch.Groups[3]
                                                                     .Value),
                                                 int.Parse(regexMatch.Groups[4]
                                                                     .Value));

                var distance = CoordinateHelper.ManhattanDistance(beaconCoord,
                                                                  sensorCoord);

                sensors.Add(new Sensor
                            {
                                X = sensorCoord.X,
                                Y = sensorCoord.Y,
                                DistanceToClosestBeacon = distance
                            });

                beacons.Add(beaconCoord);
            }
        }

        public HashSet<Coordinate> beacons;

        public HashSet<Sensor> sensors;
    }
}

public class Sensor : Coordinate
{
    public int DistanceToClosestBeacon { get; init; }

}
