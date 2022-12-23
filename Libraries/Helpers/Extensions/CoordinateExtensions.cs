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

            neighbors[(int)Direction.West] = new Coordinate(startX - 1,
                                                             startY);
            neighbors[(int)Direction.East] = new Coordinate(startX + 1,
                                                            startY);
            neighbors[(int)Direction.South] = new Coordinate(startX,
                                                             startY - 1);
            neighbors[(int)Direction.North] = new Coordinate(startX,
                                                             startY + 1);

            if (!includeDiagonals)
            {
                return neighbors.ToList();
            }

            neighbors[(int)Direction.SouthWest] = new Coordinate(startX - 1,
                                                                 startY - 1);
            neighbors[(int)Direction.NorthWest] = new Coordinate(startX - 1,
                                                                 startY + 1);
            neighbors[(int)Direction.SouthEast] = new Coordinate(startX + 1,
                                                                 startY - 1);
            neighbors[(int)Direction.NorthEast] = new Coordinate(startX + 1,
                                                                 startY + 1);

            return neighbors.ToList();
        }

        public static void Draw(this IEnumerable<Coordinate> coordinates,
                                Func<Coordinate, string> drawAction,
                                string originMarker = "O",
                                string emptySpace = ".",
                                bool forceOrigin = false)
        {
            var coordinateSet = coordinates.ToHashSet();

            var minX = coordinates.Min(x => x.X) - 1;
            var maxX = coordinates.Max(x => x.X) + 1;

            var minY = coordinates.Min(y => y.Y) - 1;
            var maxY = coordinates.Max(y => y.Y) + 1;

            if (forceOrigin)
            {
                minX = Math.Min(minX,
                                0);
                minY = Math.Min(minY,
                                0);
            }

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