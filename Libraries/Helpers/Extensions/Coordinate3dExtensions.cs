using System;

using Helpers.Maps._3D;

namespace Helpers.Extensions
{
    public static class Coordinate3dExtensions
    {
        #region Class Methods

        public static Coordinate3d[] GetNeighbors(this Coordinate3d start,
                                                  bool includeDiagonals = false)
        {
            var startX = start.X;
            var startY = start.Y;
            var startZ = start.Z;

            var neighbors = new Coordinate3d[includeDiagonals
                                                 ? 8
                                                 : 6];

            // To the left
            neighbors[0] = new Coordinate3d(startX - 1,
                                            startY,
                                            startZ);

            // To the right
            neighbors[1] = new Coordinate3d(startX + 1,
                                            startY,
                                            startZ);

            // In front of
            neighbors[2] = new Coordinate3d(startX,
                                            startY + 1,
                                            startZ);

            // Behind
            neighbors[3] = new Coordinate3d(startX,
                                            startY - 1,
                                            startZ);

            // Above
            neighbors[4] = new Coordinate3d(startX,
                                            startY,
                                            startZ + 1);

            // Below
            neighbors[5] = new Coordinate3d(startX,
                                            startY,
                                            startZ - 1);

            if (includeDiagonals)
            {
                throw new InvalidOperationException("Diagonals not coded for 3d yet.");
            }

            return neighbors;
        }

        #endregion
    }
}