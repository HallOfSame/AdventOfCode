using System;

namespace Helpers.Maps
{
    public static class CoordinateHelper
    {
        #region Class Methods

        public static int ManhattanDistance(Coordinate c1,
                                            Coordinate c2)
        {
            return Math.Abs(c1.X - c2.X) + Math.Abs(c1.Y - c2.Y);
        }

        public static Coordinate GetDirection(this Coordinate c, Direction d)
        {
            var updatedCoordinate = new Coordinate(c.X, c.Y);

            switch (d)
            {
                case Direction.North:
                    updatedCoordinate.Y += 1;
                    break;
                case Direction.South:
                    updatedCoordinate.Y -= 1;
                    break;
                case Direction.East:
                    updatedCoordinate.X += 1;
                    break;
                case Direction.West:
                    updatedCoordinate.X -= 1;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return updatedCoordinate;
        }

        #endregion
    }
}