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

            //PT 2

            var ship2 = new Part2Ship();

            foreach (var move in moves)
            {
                ship2.MoveShip(move);
            }

            var location2 = ship2.GetCurrentPosition();

            var distance2 = Math.Abs(location2.x) + Math.Abs(location2.y);

            Console.WriteLine($"Distance from start PT 2: {distance2}.");
        }

        #endregion
    }
}