using System;

namespace Day12
{
    public class Ship
    {
        #region Fields

        private MoveDirection currentFacing;

        private int xPos;

        private int yPos;

        #endregion

        #region Constructors

        public Ship()
        {
            xPos = 0;
            yPos = 0;
            currentFacing = MoveDirection.East;
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

                var currentFacingInt = (int)currentFacing;

                currentFacingInt += turnsToMake * turnModifier;

                currentFacingInt = Math.Abs(currentFacingInt);

                Math.DivRem(currentFacingInt,
                            4,
                            out currentFacingInt);

                currentFacing = (MoveDirection)currentFacingInt;
                return;
            }

            var directionToMove = move.Direction;

            if (directionToMove == MoveDirection.Forward)
            {
                directionToMove = currentFacing;
            }

            switch (directionToMove)
            {
                case MoveDirection.North:
                    xPos += move.Amount;
                    break;
                case MoveDirection.South:
                    xPos -= move.Amount;
                    break;
                case MoveDirection.East:
                    yPos += move.Amount;
                    break;
                case MoveDirection.West:
                    yPos -= move.Amount;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid move direction {directionToMove}.");
            }
        }

        #endregion
    }

    public class ShipMove
    {
        #region Constructors

        public ShipMove(MoveDirection direction,
                        int amount)
        {
            Direction = direction;
            Amount = amount;
        }

        #endregion

        #region Instance Properties

        public int Amount { get; }

        public MoveDirection Direction { get; }

        #endregion
    }

    public enum MoveDirection
    {
        Left = -1,

        Right = -2,

        Forward = -3,

        North = 0,

        South = 2,

        East = 1,

        West = 3
    }
}