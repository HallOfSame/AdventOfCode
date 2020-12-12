using System;
using System.IO;
using System.Linq;

namespace Day12
{
    internal class Program
    {
        #region Class Methods

        private static void Main(string[] args)
        {
            var fileText = File.ReadAllText("PuzzleInput.txt");

            MoveDirection GetMoveFromChar(char move)
            {
                return move switch
                {
                    'N' => MoveDirection.North,
                    'S' => MoveDirection.South,
                    'W' => MoveDirection.West,
                    'E' => MoveDirection.East,
                    'R' => MoveDirection.Right,
                    'L' => MoveDirection.Left,
                    'F' => MoveDirection.Forward,
                    _ => throw new ArgumentOutOfRangeException(nameof(move))
                };
            }

            var moves = fileText.Split(Environment.NewLine)
                                .Select(x => new ShipMove(GetMoveFromChar(x[0]),
                                                          int.Parse(x[1..])))
                                .ToList();

            var ship = new Ship();

            foreach (var move in moves)
            {
                ship.MoveShip(move);
            }

            var location = ship.GetCurrentPosition();

            var distance = Math.Abs(location.x) + Math.Abs(location.y);

            Console.WriteLine($"Distance from start: {distance}.");
        }

        #endregion
    }
}