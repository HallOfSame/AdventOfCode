namespace Helpers.Maps
{
    public class ObjectWithCoordinateEquality
    {
        #region Constructors

        public ObjectWithCoordinateEquality(Coordinate coordinate)
        {
            Coordinate = coordinate;
        }

        #endregion

        #region Instance Properties

        public Coordinate Coordinate { get; set; }

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

            return Equals((ObjectWithCoordinateEquality)obj);
        }

        public override int GetHashCode()
        {
            return Coordinate.GetHashCode();
        }

        protected bool Equals(ObjectWithCoordinateEquality other)
        {
            return Coordinate.Equals(other.Coordinate);
        }

        #endregion

        #region Class Methods

        public static bool operator ==(ObjectWithCoordinateEquality left,
                                       ObjectWithCoordinateEquality right)
        {
            return Equals(left,
                          right);
        }

        public static bool operator !=(ObjectWithCoordinateEquality left,
                                       ObjectWithCoordinateEquality right)
        {
            return !Equals(left,
                           right);
        }

        #endregion
    }
}