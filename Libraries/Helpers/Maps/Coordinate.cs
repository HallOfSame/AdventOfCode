﻿using System;

namespace Helpers.Maps
{
    public class Coordinate : ICloneable
    {
        #region Constructors

        public Coordinate()
        {
        }

        public Coordinate(decimal x,
                          decimal y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Instance Properties

        public decimal X { get; set; }

        public decimal Y { get; set; }

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

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public object Clone()
        {
            return new Coordinate(X,
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