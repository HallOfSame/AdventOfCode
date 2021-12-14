namespace Helpers.Coordinates
{
    public class Coordinate2D
    {
        #region Constructors

        public Coordinate2D()
        {
        }

        public Coordinate2D(int x,
                            int y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Instance Properties

        public int X { get; set; }

        public int Y { get; set; }

        #endregion

        #region Instance Methods

        public override bool Equals(object? obj)
        {
            return obj is Coordinate2D coordinate && X == coordinate.X && Y == coordinate.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X,
                                    Y);
        }

        #endregion
    }
}