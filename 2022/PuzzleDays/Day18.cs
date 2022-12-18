using System.Diagnostics;

using Helpers;
using Helpers.Extensions;
using Helpers.Maps._3D;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day18 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var exposedSides = 0;

            var coordHash = coordinates.ToHashSet();

            foreach (var coordinate in coordinates)
            {
                var baseExposedSides = 6;

                var coveredSides = coordinate.GetNeighbors().Count(x => coordHash.Contains(x));

                exposedSides += (baseExposedSides - coveredSides);
            }

            return exposedSides.ToString();
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var boxEdgeSize = 1;

            // Get a bounding box around our known coordinates
            var xBound = new[]
                         {
                             coordinates.Min(x => x.X) - boxEdgeSize,
                             coordinates.Max(x => x.X) + boxEdgeSize
                         };

            var yBound = new[]
                         {
                             coordinates.Min(x => x.Y) - boxEdgeSize,
                             coordinates.Max(x => x.Y) + boxEdgeSize
                         };

            var zBound = new[]
                         {
                             coordinates.Min(x => x.Z) - boxEdgeSize,
                             coordinates.Max(x => x.Z) + boxEdgeSize
                         };

            var exposedSides = 0;

            var coordHash = coordinates.ToHashSet();

            var internalCoordinates = new HashSet<Coordinate3d>();

            // Non-diagonal directions we could travel from a point
            var transforms = new[]
                             {
                                 new Coordinate3d(-1,
                                                  0,
                                                  0),
                                 new Coordinate3d(1,
                                                  0,
                                                  0),
                                 new Coordinate3d(0,
                                                  -1,
                                                  0),
                                 new Coordinate3d(0,
                                                  1,
                                                  0),
                                 new Coordinate3d(0,
                                                  0,
                                                  -1),
                                 new Coordinate3d(0,
                                                  0,
                                                  1),
                             };

            // For all coordinates in the droplet
            foreach (var coordinate in coordinates)
            {
                var outerUncoveredSides = 0;

                // Get all "neighbors", basically just the 6 directions around this coordinate
                var neighbors = coordinate.GetNeighbors();

                // From each of those neighbors, see if we can escape the droplet
                // If we can, this is one exposed face
                foreach (var startPoint in neighbors)
                {
                    // This is a known coordinate in the droplet, side is covered based on part 1 logic
                    if (coordHash.Contains(startPoint))
                    {
                        continue;
                    }

                    // We already know this coordinate is internal space and can fast skip it
                    if (internalCoordinates.Contains(startPoint))
                    {
                        continue;
                    }

                    // The main logic to find exposed faces
                    // Search for a path out of the box
                    // Sort of a djikstra where we end if coordinates hit a certain range
                    // For example:
                    // (endless void of space)
                    // #  ####
                    // #    @#
                    // #######
                    // The @ here is exposed, so we need to look at any path that gets us through the bounding box
                    bool CanEscapeBoundingBox()
                    {
                        var current = startPoint;

                        var checkQueue = new Queue<Coordinate3d>();

                        // Add the initial directions we could travel
                        transforms.Select(x => current + x)
                                  .ToList()
                                  .ForEach(x => checkQueue.Enqueue(x));

                        // Keep track of where we've been
                        var visited = new HashSet<Coordinate3d>();

                        while (checkQueue.Any())
                        {
                            var updatedPosition = checkQueue.Dequeue();

                            if (visited.Contains(updatedPosition))
                            {
                                // Already been here
                                continue;
                            }

                            visited.Add(updatedPosition);

                            if (coordHash.Contains(updatedPosition))
                            {
                                // This is a known coordinate of the droplet
                                continue;
                            }

                            // We already know this coordinate is internal space
                            if (internalCoordinates.Contains(updatedPosition))
                            {
                                continue;
                            }

                            if (updatedPosition.X < xBound[0]
                                || updatedPosition.X > xBound[1]
                                || updatedPosition.Y < yBound[0]
                                || updatedPosition.Y > yBound[1]
                                || updatedPosition.Z < zBound[0]
                                || updatedPosition.Z > zBound[1])
                            {
                                // We've escaped the bounds of our known coordinates
                                return true;
                            }

                            // Add new directions to check
                            transforms.Select(x => updatedPosition + x)
                                      .Where(x => !visited.Contains(x))
                                      .ToList()
                                      .ForEach(x => checkQueue.Enqueue(x));
                        }

                        // Ran out of places to check and never escaped the bounds, this space is internal
                        return false;
                    }

                    // Check expanding in other directions
                    // If we can expand out of the bounding box without a collision this is an outer exposed edge
                    if (CanEscapeBoundingBox())
                    {
                        outerUncoveredSides++;
                        continue;
                    }

                    // Track if we know a coordinate is an internal space so we can skip processing it later
                    internalCoordinates.Add(startPoint);
                }

                exposedSides += outerUncoveredSides;
            }

            // Got an oddly narrow range when code still had bugs
            if (exposedSides <= 2551)
            {
                throw new Exception("Too low");
            }

            if (exposedSides >= 2583)
            {
                throw new Exception("Too high");
            }

            return exposedSides.ToString();
        }

        public override async Task ReadInput()
        {
            coordinates = await new CoordinateReader().ReadInputFromFile();
        }

        private List<Coordinate3d> coordinates;
    }
}

class CoordinateReader : FileReader<Coordinate3d>
{
    protected override Coordinate3d ProcessLineOfFile(string line)
    {
        var split = line.Split(',');

        return new Coordinate3d(int.Parse(split[0]),
                                int.Parse(split[1]),
                                int.Parse(split[2]));
    }
}