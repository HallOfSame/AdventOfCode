using System;
using System.Collections.Generic;
using System.IO;

namespace Day24
{
    internal class Program
    {
        #region Class Methods

        private static void Main(string[] args)
        {
            var fileLines = File.ReadAllLines("PuzzleInput.txt");

            var blackTiles = new HashSet<(int, int)>();

            var moveList = new List<List<Move>>();

            foreach (var line in fileLines)
            {
                var movesForThisLine = new List<Move>();

                var charArray = line.ToCharArray();

                for (var i = 0; i < charArray.Length; i++)
                {
                    switch (charArray[i])
                    {
                        case 'e':
                            movesForThisLine.Add(Move.East);
                            break;
                        case 'w':
                            movesForThisLine.Add(Move.West);
                            break;
                        case 's':
                            movesForThisLine.Add(charArray[i + 1] == 'e'
                                                     ? Move.SouthEast
                                                     : Move.SouthWest);

                            i++;
                            break;
                        case 'n':
                            movesForThisLine.Add(charArray[i + 1] == 'e'
                                                     ? Move.NorthEast
                                                     : Move.NorthWest);

                            i++;
                            break;
                    }
                }

                moveList.Add(movesForThisLine);
            }

            (int, int) GetCoordinatesForMove(List<Move> moveSet)
            {
                // Using axial coordinates
                var r = 0;
                var q = 0;

                foreach (var move in moveSet)
                {
                    switch (move)
                    {
                        case Move.NorthEast:
                            r -= 1;
                            q += 1;
                            break;
                        case Move.NorthWest:
                            r -= 1;
                            break;
                        case Move.East:
                            q += 1;
                            break;
                        case Move.West:
                            q -= 1;
                            break;
                        case Move.SouthEast:
                            r += 1;
                            break;
                        case Move.SouthWest:
                            q -= 1;
                            r += 1;
                            break;
                    }
                }

                return (q, r);
            }

            var blackFlips = 0;
            var flipBacks = 0;

            foreach (var moveSet in moveList)
            {
                var coordinates = GetCoordinatesForMove(moveSet);

                if (blackTiles.Contains(coordinates))
                {
                    flipBacks++;
                    blackTiles.Remove(coordinates);
                }
                else
                {
                    blackFlips++;
                    blackTiles.Add(coordinates);
                }
            }

            Console.WriteLine($"Black tiles after moves: {blackTiles.Count}.");
        }

        #endregion
    }

    /// <summary>
    /// The six directions we can move from a tile
    /// </summary>
    public enum Move
    {
        East,

        SouthEast,

        SouthWest,

        West,

        NorthWest,

        NorthEast
    }

    internal class Tile
    {
        #region Constructors

        public Tile(int q,
                    int r)
        {
            Q = q;
            R = r;
        }

        #endregion

        #region Instance Properties

        public int Q { get; }

        public int R { get; }

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

            return Equals((Tile)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Q,
                                    R);
        }

        protected bool Equals(Tile other)
        {
            return Q == other.Q && R == other.R;
        }

        #endregion
    }
}