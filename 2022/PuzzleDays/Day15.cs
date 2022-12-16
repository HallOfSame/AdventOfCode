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
            var searchArea = new Quadrant(0,
                                          4_000_000,
                                          0,
                                          4_000_000);

            List<Quadrant> SplitQuadrantToFourSections(Quadrant q)
            {
                var midX = (q.MaxX - q.MinX) / 2;
                var midY = (q.MaxY - q.MinY) / 2;

                var upperLeft = new Quadrant(q.MinX,
                                             q.MinX + midX,
                                             q.MinY + midY + 1,
                                             q.MaxY);

                var upperRight = new Quadrant(q.MinX + midX + 1,
                                              q.MaxX,
                                              q.MinY + midY + 1,
                                              q.MaxY);

                var lowerLeft = new Quadrant(q.MinX,
                                             q.MinX + midX,
                                             q.MinY,
                                             q.MinY + midY);

                var lowerRight = new Quadrant(q.MinX + midX + 1,
                                              q.MaxX,
                                              q.MinY,
                                              q.MinY + midY);

                return new List<Quadrant>
                       {
                           upperLeft,
                           upperRight,
                           lowerLeft,
                           lowerRight
                       };
            }

            var quadrantsToCheck = new Queue<Quadrant>();

            quadrantsToCheck.Enqueue(searchArea);

            Coordinate distressBeacon = null;

            while (quadrantsToCheck.Any())
            {
                var current = quadrantsToCheck.Dequeue();

                if (current.MinX == current.MaxX)
                {
                    distressBeacon = current.Corners.First();
                    break;
                }

                var subSections = SplitQuadrantToFourSections(current);

                subSections.Where(x => x.CanContainMissingCoordinate(sensors))
                           .ToList()
                           .ForEach(x => quadrantsToCheck.Enqueue(x));
            }

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

public class Quadrant
{
    public int MinX { get; }

    public int MaxX { get; }

    public int MinY { get; }

    public int MaxY { get; }

    public List<Coordinate> Corners { get; }

    public Quadrant(int minX,
                    int maxX,
                    int minY,
                    int maxY)
    {
        if (minY > maxY)
        {
            throw new Exception();
        }

        MinX = minX;
        MaxX = maxX;
        MinY = minY;
        MaxY = maxY;
        Corners = new List<Coordinate>
                  {
                      new(minX,
                          maxY),
                      new(minX,
                          minY),
                      new(maxX,
                          maxY),
                      new(maxX,
                          minY)
                  };
    }

    public bool CanContainMissingCoordinate(IEnumerable<Sensor> sensors)
    {
        return sensors.All(sensor =>
                           {
                               var maxDistance = 0;

                               for (var i = 0; i < 4; i++)
                               {
                                   maxDistance = Math.Max(CoordinateHelper.ManhattanDistance(Corners[i],
                                                                                             sensor),
                                                          maxDistance);
                               }

                               return maxDistance > sensor.DistanceToClosestBeacon;
                           });
    }
}

public class Sensor : Coordinate
{
    public int DistanceToClosestBeacon { get; init; }

}
