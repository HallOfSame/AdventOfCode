using System;

namespace Helpers.Maps._3D
{
    public class Coordinate3d : Coordinate
    {
        #region Constructors

        public Coordinate3d()
        {
        }

        public Coordinate3d(decimal x,
                            decimal y,
                            decimal z)
            : base(x,
                   y)
        {
            Z = z;
        }

        #endregion

        #region Instance Properties

        public decimal Z { get; set; }

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

            return Equals((Coordinate3d)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(),
                                    Z);
        }

        public override string ToString()
        {
            return $"({this.X}, {this.Y}, {Z})";
        }

        protected bool Equals(Coordinate3d other)
        {
            return base.Equals(other) && Z == other.Z;
        }

        #endregion

        #region Class Methods

        public static Coordinate3d operator +(Coordinate3d a,
                                              Coordinate3d b)
        {
            return new Coordinate3d
                   {
                       X = a.X + b.X,
                       Y = a.Y + b.Y,
                       Z = a.Z + b.Z
                   };
        }

        public static Coordinate3d operator -(Coordinate3d a,
                                              Coordinate3d b)
        {
            return new Coordinate3d
                   {
                       X = a.X - b.X,
                       Y = a.Y - b.Y,
                       Z = a.Z - b.Z
                   };
        }

        #endregion
    }
}