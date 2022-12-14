using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day14 : ProblemBase
    {
        private int partOneResult;

        protected override async Task<string> SolvePartOneInternal()
        {
            partOneResult = map.GetAmountOfSandThatComesToRest(false);

            return partOneResult.ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            // Optimized to avoid duplicate calculations
            // Definitely not me not wanting to avoid the side effects in the existing class
            return (map.GetAmountOfSandThatComesToRest(true) + partOneResult).ToString();
        }

        public override async Task ReadInput()
        {
            var lines = await new StringFileReader().ReadInputFromFile();

            map = new Map();

            var filledCoordinates = new HashSet<Coordinate>();

            foreach (var line in lines)
            {
                var coordinates = line.Split("->")
                                      .Select(x => x.Trim())
                                      .Select(x =>
                                              {
                                                  var split = x.Split(",");

                                                  return new Coordinate(int.Parse(split[0]),
                                                                        int.Parse(split[1]));
                                              })
                                      .ToList();

                var current = coordinates.First();

                foreach (var nextCoordinate in coordinates.Skip(1))
                {
                    do
                    {
                        filledCoordinates.Add((Coordinate)current.Clone());

                        if (current.X == nextCoordinate.X)
                        {
                            // Vertical
                            current.Y = nextCoordinate.Y > current.Y
                                            ? current.Y + 1
                                            : current.Y - 1;
                        }
                        else
                        {
                            // Horizontal
                            current.X = nextCoordinate.X > current.X
                                            ? current.X + 1
                                            : current.X - 1;
                        }
                    }
                    while (current != nextCoordinate);
                }

                filledCoordinates.Add(coordinates.Last());
            }

            map.FilledLocations = filledCoordinates.ToDictionary(x => x,
                                                                 x => x);
        }

        private Map map;
    }
}

class Map
{
    public Dictionary<Coordinate, Coordinate> FilledLocations { get; set; } = new();

    public Coordinate SandSource { get; set; } = new(500,
                                                     0);

    public int GetAmountOfSandThatComesToRest(bool part2)
    {
        // Coordinates are weird here, 0 Y is the highest, and sand "falls" in an increasing Y
        // If we get below this rock, it will fall forever and we can stop
        var lowestRockY = FilledLocations.Max(x => x.Value.Y);

        var caveFloor = lowestRockY + 2;

        var sandAtRest = 0;

        Coordinate? currentMovingSand = null;

        while (true)
        {
            // Reset if we can start a new grain
            currentMovingSand ??= (Coordinate)SandSource.Clone();

            bool CanMoveTo(int x,
                           int y)
            {
                if (part2 && y == caveFloor)
                {
                    return false;
                }

                return !FilledLocations.ContainsKey(new Coordinate(x,
                                                                   y));
            }

            if (!part2)
            {
                if (currentMovingSand.Y > lowestRockY)
                {
                    // Starting to fall forever
                    return sandAtRest;
                }
            }

            if (CanMoveTo(currentMovingSand.X,
                          currentMovingSand.Y + 1))
            {
                currentMovingSand.Y++;
            }
            else if (CanMoveTo(currentMovingSand.X - 1,
                                  currentMovingSand.Y + 1))
            {
                currentMovingSand.X--;
                currentMovingSand.Y++;
            }
            else if (CanMoveTo(currentMovingSand.X + 1,
                               currentMovingSand.Y + 1))
            {
                currentMovingSand.X++;
                currentMovingSand.Y++;
            }
            else
            {
                // Nowhere to move
                sandAtRest++;

                if (currentMovingSand == SandSource)
                {
                    return sandAtRest;
                }

                FilledLocations.Add(currentMovingSand,
                                    currentMovingSand);
                
                currentMovingSand = null;
            }
        }
    }
}