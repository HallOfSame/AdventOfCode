using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

            // PT 1
            Console.WriteLine($"Black tiles after moves: {blackTiles.Count}.");


            HashSet<(int, int)> GetNeighbors((int, int) tile)
            {
                var (q, r) = tile;

                return new HashSet<(int, int)>
                       {
                           (q, r - 1),
                           (q + 1, r - 1),
                           (q + 1, r),
                           (q, r + 1),
                           (q - 1, r + 1),
                           (q - 1, r)
                       };
            }

            var daysToProcess = 100;

            var currentBlackTiles = new HashSet<(int, int)>(blackTiles);

            var nextDayBlackTiles = new HashSet<(int, int)>(currentBlackTiles.Count);

            for (var i = 1; i <= daysToProcess; i++)
            {
                var tilesToCheck = currentBlackTiles.SelectMany(x => GetNeighbors(x))
                                                    .Concat(currentBlackTiles)
                                                    .Distinct()
                                                    .ToList();

                foreach(var tile in tilesToCheck)
                {
                    var adjacentBlackTiles = GetNeighbors(tile)
                        .Count(x => currentBlackTiles.Contains(x));

                    bool isBlackNextDay;

                    if (currentBlackTiles.Contains(tile))
                    {
                        isBlackNextDay = !(adjacentBlackTiles == 0 || adjacentBlackTiles > 2);
                    }
                    else
                    {
                        isBlackNextDay = adjacentBlackTiles == 2;
                    }

                    if (isBlackNextDay)
                    {
                        nextDayBlackTiles.Add(tile);
                    }
                }

                currentBlackTiles = new HashSet<(int, int)>(nextDayBlackTiles);
                nextDayBlackTiles.Clear();
            }

            Console.WriteLine($"Black tiles after {daysToProcess} days of moves: {currentBlackTiles.Count}.");
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
}