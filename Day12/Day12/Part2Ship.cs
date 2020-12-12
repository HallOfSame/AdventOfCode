using System;
using System.Collections.Generic;
using System.Text;

namespace Day12
{

    public class Part2Ship
    {
        #region Fields

        private int xPos;

        private int yPos;

        private int waypointX;

        private int waypointY;

        #endregion

        #region Constructors

        public Part2Ship()
        {
            xPos = 0;
            yPos = 0;
            // Waypoint starts 10 unit E and 1 unit north
            waypointX = 1; 
            waypointY = 10;
        }

        #endregion

        #region Instance Methods

        public (int x, int y) GetCurrentPosition()
        {
            return (xPos, yPos);
        }

        public void MoveShip(ShipMove move)
        {
            if (move.Direction == MoveDirection.Left
                || move.Direction == MoveDirection.Right)
            {
                var turnsToMake = move.Amount / 90;

                int turnModifier;

                if (move.Direction == MoveDirection.Left)
                {
                    // 3 rights make a left
                    turnModifier = 3;
                }
                else
                {
                    turnModifier = 1;
                }

                var rightTurns = turnsToMake * turnModifier;

                for (var i = 0; i < rightTurns; i++)
                {
                    var yTemp = waypointY;

                    // Y becomes the previous x
                    waypointY = waypointX;

                    // X becomes y * -1
                    waypointX = yTemp * -1;

                    //WayX = 4 N
                    //WayY = 10 E

                    //One right turn means we go to
                    //WayY = 4 E (prev way X)
                    //WayX = -10 S (prev way Y * -1)

                    //Another right should be
                    //Way Y = -10 W (prev way X)
                    //Way X = -4 S (prev way Y * -1)

                    //A third right should be
                    //Way Y = -4 W (prev way X)
                    //Way X = 10 N (prev way Y * -1)

                    //Fourth right (back to start)
                    //Way Y = 10 E (prev way X)
                    //Way X = 4 N (prev way Y * -1)

                    //Way X -> 4  -> -10 -> -4  -> 10 -> 4
                    //Way Y -> 10 -> 4   -> -10 -> -4 -> 10 
                }

                return;
            }

            if (move.Direction == MoveDirection.Forward)
            {
                // Move towards the waypoint
                xPos += waypointX * move.Amount;
                yPos += waypointY * move.Amount;
                return;
            }

            // Move the waypoint
            switch (move.Direction)
            {
                case MoveDirection.North:
                    waypointX += move.Amount;
                    break;
                case MoveDirection.South:
                    waypointX -= move.Amount;
                    break;
                case MoveDirection.East:
                    waypointY += move.Amount;
                    break;
                case MoveDirection.West:
                    waypointY -= move.Amount;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid move direction {move.Direction}.");
            }
        }

        #endregion
    }
}
