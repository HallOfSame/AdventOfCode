using System.Collections.Generic;
using System.Linq;

using Helpers.Maps;

namespace Helpers.Extensions
{
    public static class CoordinateExtensions
    {
        #region Class Methods

        public static List<Coordinate> GetNeighbors(this Coordinate start,
                                                    bool includeDiagonals = false)
        {
            var startX = start.X;
            var startY = start.Y;

            var neighbors = new Coordinate[includeDiagonals
                                               ? 8
                                               : 4];

            neighbors[0] = new Coordinate(startX - 1,
                                          startY);
            neighbors[1] = new Coordinate(startX + 1,
                                          startY);
            neighbors[2] = new Coordinate(startX,
                                          startY - 1);
            neighbors[3] = new Coordinate(startX,
                                          startY + 1);

            if (!includeDiagonals)
            {
                return neighbors.ToList();
            }

            neighbors[4] = new Coordinate(startX - 1,
                                          startY - 1);
            neighbors[5] = new Coordinate(startX - 1,
                                          startY + 1);
            neighbors[6] = new Coordinate(startX + 1,
                                          startY - 1);
            neighbors[7] = new Coordinate(startX + 1,
                                          startY + 1);

            return neighbors.ToList();
        }

        #endregion
    }
}