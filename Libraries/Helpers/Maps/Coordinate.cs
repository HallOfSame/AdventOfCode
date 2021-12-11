using System;

namespace Helpers.Maps
{
    public class Coordinate
    {
        #region Constructors

        public Coordinate()
        {
        }

        public Coordinate(int x,
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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null,
                                obj))
            {
                return false;
            }

            if (ReferenceEquals(this,
                                obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Coordinate)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X,
                                    Y);
        }

        protected bool Equals(Coordinate other)
        {
            return X == other.X && Y == other.Y;
        }

        #endregion

        #region Class Methods

        public static bool operator ==(Coordinate left,
                                       Coordinate right)
        {
            return Equals(left,
                          right);
        }

        public static bool operator !=(Coordinate left,
                                       Coordinate right)
        {
            return !Equals(left,
                           right);
        }

        #endregion
    }
}