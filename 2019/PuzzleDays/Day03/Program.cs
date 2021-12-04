using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day03
{
    public class Program
    {
        #region Class Methods

        public static void Main(string[] args)
        {
            var fileText = File.ReadAllText("PuzzleInput.txt");

            var fileSplit = fileText.Split(Environment.NewLine);

            var wireOne = fileSplit[0]
                .Split(',');

            var wireTwo = fileSplit[1]
                .Split(',');

            var wireOneCoordinates = GetCoordinatesForWire(wireOne);

            var wireTwoCoordinates = GetCoordinatesForWire(wireTwo);

            var wOHash = wireOneCoordinates.ToHashSet();
            var wTHash = wireTwoCoordinates.ToHashSet();

            var intersections = wOHash.Where(x => wTHash.Contains(x) && !(x.X == 0 && x.Y == 0))
                                      .Distinct()
                                      .ToList();

            var closestIntersect = intersections.Min(x => Math.Abs(x.X) + Math.Abs(x.Y));

            // PT 1
            Console.WriteLine($"Closest MH distance: {closestIntersect}.");


            // PT 2
            var bestSteps = int.MaxValue;

            foreach(var intersect in intersections)
            {
                // Add one cause we left out the origin point
                var stepsOne = wireOneCoordinates.IndexOf(intersect) + 1;
                var stepsTwo = wireTwoCoordinates.IndexOf(intersect) + 1;

                var stepsForThisIntersect = stepsOne + stepsTwo;

                if (stepsForThisIntersect < bestSteps)
                {
                    bestSteps = stepsForThisIntersect;
                }
            }

            Console.WriteLine($"Best steps we can use: {bestSteps}.");
        }

        private static IEnumerable<int> CreateRange(int start,
                                                    int end)
        {
            var increment = end > start
                                ? 1
                                : -1;

            for (var i = start; i != end; i += increment)
            {
                yield return i;
            }

            yield return end;
        }

        private static List<Coordinate> GetCoordinatesForWire(string[] wire)
        {
            var wireCoordinates = new List<Coordinate>();

            var wireCurrentCoord = new Coordinate(0,
                                                  0);

            foreach (var move in wire)
            {
                var direction = move[0];

                var distance = int.Parse(move.Substring(1));

                switch (direction)
                {
                    case 'U':
                        wireCoordinates.AddRange(Enumerable.Range(wireCurrentCoord.X + 1,
                                                                  distance)
                                                           .Select(x => new Coordinate(x,
                                                                                       wireCurrentCoord.Y)));
                        break;
                    case 'D':
                        wireCoordinates.AddRange(CreateRange(wireCurrentCoord.X - 1,
                                                             wireCurrentCoord.X - distance)
                                                     .Select(x => new Coordinate(x,
                                                                                 wireCurrentCoord.Y)));
                        break;
                    case 'R':
                        wireCoordinates.AddRange(Enumerable.Range(wireCurrentCoord.Y + 1,
                                                                  distance)
                                                           .Select(x => new Coordinate(wireCurrentCoord.X,
                                                                                       x)));
                        break;
                    case 'L':
                        wireCoordinates.AddRange(CreateRange(wireCurrentCoord.Y - 1,
                                                             wireCurrentCoord.Y - distance)
                                                     .Select(x => new Coordinate(wireCurrentCoord.X,
                                                                                 x)));
                        break;
                }

                wireCurrentCoord = wireCoordinates.Last();
            }

            return wireCoordinates;
        }

        #endregion
    }

    [DebuggerDisplay("{X}, {Y}")]
    public class Coordinate
    {
        #region Constructors

        public Coordinate(int x,
                          int y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Instance Properties

        public int X { get; }

        public int Y { get; }

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
    }
}