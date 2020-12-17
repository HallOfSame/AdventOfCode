using System.Collections.Generic;
using System.Linq;

namespace Day17
{
    public static class Simulator
    {
        #region Class Methods

        public static void RunOneTurn(Dimension input)
        {
            var newActive = new HashSet<Coordinate>();

            // Get the possible shifting cubes, neighbors of all the active cubes
            var coordinatesToProcess = input.ActiveCoordinates.Concat(input.ActiveCoordinates.SelectMany(x => x.GetNeighbors))
                                            .Distinct()
                                            .ToList();

            foreach (var coordinate in coordinatesToProcess)
            {
                var activeNeighbors = input.ActiveCoordinates.Intersect(coordinate.GetNeighbors)
                                           .Count();

                var isActive = input.ActiveCoordinates.Contains(coordinate);

                if (isActive && activeNeighbors == 2)
                {
                    // Active w/ 2 active neighbors remains
                    newActive.Add(coordinate);
                }
                else if (activeNeighbors == 3)
                {
                    // If active with 3 actives it stays
                    // If inactive with 3 it becomes active
                    newActive.Add(coordinate);
                }
            }

            input.ActiveCoordinates = newActive;
        }

        #endregion
    }
}