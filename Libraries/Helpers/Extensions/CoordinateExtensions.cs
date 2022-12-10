using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static void Draw(this IEnumerable<Coordinate> coordinates,
                                Func<Coordinate, string> drawAction,
                                string originMarker = "O",
                                string emptySpace = ".")
        {
            var coordinateSet = coordinates.ToHashSet();

            var minX = coordinates.Min(x => x.X) - 1;
            var maxX = coordinates.Max(x => x.X) + 1;

            var minY = coordinates.Min(y => y.Y) - 1;
            var maxY = coordinates.Max(y => y.Y) + 1;

            var sb = new StringBuilder();

            // Going in reverse order for Y draws it in the way you'd expect
            for (var y = maxY; y >= minY; y--)
            {
                for (var x = minX; x <= maxX; x++)
                {
                    if (coordinateSet.TryGetValue(new Coordinate(x,
                                                                 y),
                                                  out var coordinate))
                    {
                        sb.Append(drawAction(coordinate));
                    }
                    else if (x == 0
                             && y == 0)
                    {
                        sb.Append(originMarker);
                    }
                    else
                    {
                        sb.Append(emptySpace);
                    }
                }

                sb.Append(Environment.NewLine);
            }

            Console.WriteLine(sb.ToString());
        }

        #endregion
    }
}