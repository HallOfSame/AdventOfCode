using System.Text.RegularExpressions;

using Helpers.Extensions;
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

                var couldNotHaveBeacon = IsInvalidBeaconLocation(testCoordinate);

                if (couldNotHaveBeacon)
                {
                    impossibleBeaconLocations++;
                }
            }

            if (impossibleBeaconLocations != 5240818)
            {
                // For refactoring
                throw new Exception("You broke it");
            }

            return impossibleBeaconLocations.ToString();
        }

        private bool IsInvalidBeaconLocation(Coordinate testLocation)
        {
            return sensors.Any(x => CoordinateHelper.ManhattanDistance(x,
                                                                       testLocation)
                                    <= x.DistanceToClosestBeacon);
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var possibleLocationsPerSensor = sensors.ToList()
                                                    .AsParallel()
                                                    .Select((sensor, idx) =>
                                                            {
                                                                Console.WriteLine($"Processing sensor number: {idx + 1} of {sensors.Count}");
                                                                var queue = new Queue<Coordinate>();

                                                                queue.Enqueue(new Coordinate(sensor.X,
                                                                                             sensor.Y + sensor.DistanceToClosestBeacon));

                                                                queue.Enqueue(new Coordinate(sensor.X, sensor.Y - sensor.DistanceToClosestBeacon));

                                                                queue.Enqueue(new Coordinate(sensor.X + sensor.DistanceToClosestBeacon,
                                                                                             sensor.Y));

                                                                queue.Enqueue(new Coordinate(sensor.X - sensor.DistanceToClosestBeacon,
                                                                                             sensor.Y));

                                                                var result = new HashSet<Coordinate>();

                                                                var visited = new HashSet<Coordinate>();

                                                                while (queue.Any())
                                                                {
                                                                    var current = queue.Dequeue();

                                                                    visited.Add(current);

                                                                    var neighborsToCheck = current.GetNeighbors(true)
                                                                                                  .Where(x => x.X is >= 0 and <= 4_000_000 && x.Y is >= 0 and <= 4_000_000 && !visited.Contains(x));

                                                                    foreach (var neighbor in neighborsToCheck)
                                                                    {
                                                                        var distanceToSensor = CoordinateHelper.ManhattanDistance(sensor,
                                                                                                                                  neighbor);

                                                                        var diff = distanceToSensor - sensor.DistanceToClosestBeacon;

                                                                        if (diff == 1)
                                                                        {
                                                                            // If it is exactly one outside of the closest, it is a possible result
                                                                            if (!IsInvalidBeaconLocation(neighbor))
                                                                            {
                                                                                result.Add(neighbor);
                                                                            }
                                                                        }
                                                                        else if (diff is 0 or -1)
                                                                        {
                                                                            // Otherwise, keep searching if it is around the borders
                                                                            // Add to visited here to prevent duplicate adds
                                                                            visited.Add(neighbor);
                                                                            queue.Enqueue(neighbor);
                                                                        }
                                                                    }
                                                                }

                                                                return result;
                                                            })
                                                    .Aggregate((curr,
                                                                prev) =>
                                                               {
                                                                   curr.UnionWith(prev);

                                                                   return curr;
                                                               });

            var distressBeacon = possibleLocationsPerSensor.First();

            var result = ((distressBeacon.X * 4_000_000m) + distressBeacon.Y);

            if (result != 13213086906101m)
            {
                // For refactoring
                throw new Exception("You broke it");
            }

            return result.ToString();
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
