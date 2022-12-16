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

        #endregion
    }
}