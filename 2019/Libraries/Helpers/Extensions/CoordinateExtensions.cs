using Helpers.Coordinates;

namespace Helpers.Extensions
{
    public static class CoordinateExtensions
    {
        #region Class Methods

        public static List<T> GetNeighborCoordinates<T>(this T coordinate)
            where T : Coordinate2D, new()
        {
            return new List<T>
                   {
                       new()
                       {
                           X = coordinate.X - 1,
                           Y = coordinate.Y
                       },
                       new()
                       {
                           X = coordinate.X + 1,
                           Y = coordinate.Y
                       },
                       new()
                       {
                           X = coordinate.X,
                           Y = coordinate.Y - 1
                       },
                       new()
                       {
                           X = coordinate.X,
                           Y = coordinate.Y + 1
                       }
                   };
        }

        #endregion
    }
}