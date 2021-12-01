using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Day17
{
    public abstract class Coordinate
    {
        #region Instance Properties

        public abstract HashSet<Coordinate> GetNeighbors { get; }

        #endregion
    }

    [DebuggerDisplay("{X} {Y} {Z}")]
    public class Coordinate3d : Coordinate
    {
        #region Constructors

        public Coordinate3d(int x,
                            int y,
                            int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        #endregion

        #region Instance Properties

        public override HashSet<Coordinate> GetNeighbors
        {
            get
            {
                var neighborCoordinates = new HashSet<Coordinate>(26);

                for (var x = -1; x <= 1; x++)
                {
                    for (var y = -1; y <= 1; y++)
                    {
                        for (var z = -1; z <= 1; z++)
                        {
                            if (x == 0
                                && y == 0
                                && z == 0)
                            {
                                // Don't add this current one to its own neighbor list
                                continue;
                            }

                            neighborCoordinates.Add(Shift(x,
                                                          y,
                                                          z));
                        }
                    }
                }

                return neighborCoordinates;
            }
        }

        public int X { get; }

        public int Y { get; }

        public int Z { get; }

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
            return HashCode.Combine(X,
                                    Y,
                                    Z);
        }

        public Coordinate3d Shift(int x,
                                  int y,
                                  int z)
        {
            return new Coordinate3d(X + x,
                                    Y + y,
                                    Z + z);
        }

        protected bool Equals(Coordinate3d other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        #endregion
    }

    [DebuggerDisplay("{X} {Y} {Z} {W}")]
    public class Coordinate4d : Coordinate
    {
        #region Constructors

        public Coordinate4d(int x,
                            int y,
                            int z,
                            int w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        #endregion

        #region Instance Properties

        public override HashSet<Coordinate> GetNeighbors
        {
            get
            {
                var neighborCoordinates = new HashSet<Coordinate>(88);

                for (var x = -1; x <= 1; x++)
                {
                    for (var y = -1; y <= 1; y++)
                    {
                        for (var z = -1; z <= 1; z++)
                        {
                            for (var w = -1; w <= 1; w++)
                            {
                                if (x == 0
                                    && y == 0
                                    && z == 0
                                    && w == 0)
                                {
                                    // Don't add this current one to its own neighbor list
                                    continue;
                                }

                                neighborCoordinates.Add(Shift(x,
                                                              y,
                                                              z,
                                                              w));
                            }
                        }
                    }
                }

                return neighborCoordinates;
            }
        }

        public int W { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

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

            return Equals((Coordinate4d)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(W,
                                    X,
                                    Y,
                                    Z);
        }

        public Coordinate4d Shift(int x,
                                  int y,
                                  int z,
                                  int w)
        {
            return new Coordinate4d(X + x,
                                    Y + y,
                                    Z + z,
                                    W + w);
        }

        protected bool Equals(Coordinate4d other)
        {
            return W == other.W && X == other.X && Y == other.Y && Z == other.Z;
        }

        #endregion
    }
}