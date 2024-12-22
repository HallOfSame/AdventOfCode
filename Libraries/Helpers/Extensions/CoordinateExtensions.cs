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

            AddDirectionToList(Direction.West);
            AddDirectionToList(Direction.East);
            AddDirectionToList(Direction.South);
            AddDirectionToList(Direction.North);

            if (!includeDiagonals)
            {
                return neighbors.ToList();
            }

            AddDirectionToList(Direction.SouthWest);
            AddDirectionToList(Direction.NorthWest);
            AddDirectionToList(Direction.SouthEast);
            AddDirectionToList(Direction.NorthEast);

            return neighbors.ToList();

            void AddDirectionToList(Direction direction)
            {
                neighbors[(int)direction] = Move(start, direction);
            }
        }

        public static Coordinate Move(this Coordinate startCoordinate, Direction direction, int distance = 1)
        {
            var startX = startCoordinate.X;
            var startY = startCoordinate.Y;

            return direction switch
            {
                Direction.West => new Coordinate(startX - distance, startY),
                Direction.East => new Coordinate(startX + distance, startY),
                Direction.South => new Coordinate(startX, startY - distance),
                Direction.North => new Coordinate(startX, startY + distance),
                Direction.SouthWest => new Coordinate(startX - distance, startY - distance),
                Direction.NorthWest => new Coordinate(startX - distance, startY + distance),
                Direction.SouthEast => new Coordinate(startX + distance, startY - distance),
                Direction.NorthEast => new Coordinate(startX + distance, startY + distance),
                _ => throw new ArgumentException("Direction was not valid", nameof(direction))
            };
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