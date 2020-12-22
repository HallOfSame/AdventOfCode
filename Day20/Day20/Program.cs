using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day20
{
    public class Program
    {
        #region Class Methods

        public static T[,] FlipImage<T>(T[,] oldMatrix)
        {
            var newMatrix = new T[oldMatrix.GetLength(1), oldMatrix.GetLength(0)];

            var totalRows = oldMatrix.GetLength(0);
            var totalColumns = oldMatrix.GetLength(1);

            for (var oldColumn = 0; oldColumn < totalColumns; oldColumn++)
            {
                var newColumn = totalColumns - oldColumn - 1;

                for (var oldRow = 0; oldRow < totalRows; oldRow++)
                {
                    newMatrix[oldRow,
                              newColumn] = oldMatrix[oldRow,
                                                     oldColumn];
                }
            }

            return newMatrix;
        }

        public static void Main()
        {
            var fileLines = File.ReadAllLines("PuzzleInput.txt");

            var tiles = ParseTiles(fileLines);

            var matchingTiles = new Dictionary<long, HashSet<long>>();

            // Now match up sides
            foreach (var tile in tiles)
            {
                foreach (var edge in tile.Edges)
                {
                    // Check the reverse because they swap when rotated
                    var matchingIds = tiles.Where(x => x.Id != tile.Id
                                                       && x.Edges.Any(e => edge == e
                                                                           || edge
                                                                           == new string(e.Reverse()
                                                                                          .ToArray())))
                                           .Select(x => x.Id)
                                           .ToList();

                    if (!matchingTiles.ContainsKey(tile.Id))
                    {
                        matchingTiles[tile.Id] = new HashSet<long>();
                    }

                    matchingIds.ForEach(x => matchingTiles[tile.Id]
                                            .Add(x));
                }
            }

            // Corners are ones w/ 2 matches
            var corners = matchingTiles.Where(x => x.Value.Count == 2)
                                       .ToList();

            Debug.Assert(corners.Count == 4);

            // PT 1 7901522557967
            Console.WriteLine($"Corner IDs multiplied: {corners.Select(x => x.Key).Aggregate(1L, (x, y) => x * y)}");

            // Put the tiles together to form an actual image
            var image = BuildTileImage(tiles,
                                       out var size);

            // This makes the example match their layout
            // Not actually needed

            // Rotate left 90 3 times
            image = RotateMatrixCounterClockwise(image);
            image = RotateMatrixCounterClockwise(image);
            image = RotateMatrixCounterClockwise(image);
            // Flip horizontal
            image = FlipImage(image);

            DrawTileImage(image);

            // Now we can create a stitched image
            var singleImageSize = tiles[0]
                                  .Image.Count
                                  - 2;

            var stitchedImageSize = singleImageSize * size;

            var stitchedImage = GetStitchedImage(stitchedImageSize,
                                                 image,
                                                 singleImageSize);

            DrawImage(stitchedImage);

            var seaMonsterL1 = "                  # ";
            var seaMonsterL2 = "#    ##    ##    ###";
            var seaMonsterL3 = " #  #  #  #  #  #   ";

            var seaMonsterPattern = new List<string>
                                    {
                                        seaMonsterL1,
                                        seaMonsterL2,
                                        seaMonsterL3
                                    };

            var length = seaMonsterPattern[0]
                .Length;
            var height = seaMonsterPattern.Count;

            var x = stitchedImage.GetLength(0);
            var y = stitchedImage.GetLength(1);

            // Starts at the image [x,y] and checking if we can match our pattern
            bool MatchesSeaMonster(int xCo,
                                   int yCo,
                                   out HashSet<(int, int)> coordinatesUsedToMatch)
            {
                coordinatesUsedToMatch = new HashSet<(int, int)>();

                foreach (var line in seaMonsterPattern)
                {
                    foreach (var smChar in line)
                    {
                        if (smChar == '#')
                        {
                            if (stitchedImage[xCo,
                                              yCo]
                                != '#'
                                && stitchedImage[xCo,
                                                 yCo]
                                != 'O')
                            {
                                return false;
                            }

                            coordinatesUsedToMatch.Add((xCo, yCo));
                        }

                        yCo++;
                    }

                    yCo -= line.Length;
                    xCo++;
                }

                return true;
            }

            void CheckOrientationsForMonster()
            {
                for (var r = 0; r < 4; r++)
                {
                    for (var i = 0; i < x - height; i++)
                    {
                        for (var j = 0; j < y - length; j++)
                        {
                            if (MatchesSeaMonster(i,
                                                  j,
                                                  out var coordinatesUsedToMatch))
                            {
                                foreach (var coord in coordinatesUsedToMatch)
                                {
                                    stitchedImage[coord.Item1,
                                                  coord.Item2] = 'O';
                                }
                            }
                        }
                    }

                    stitchedImage = RotateMatrixCounterClockwise(stitchedImage);
                }
            }

            // Check each orientation
            CheckOrientationsForMonster();

            // Flip the image
            stitchedImage = FlipImage(stitchedImage);

            // Check each orientation again
            CheckOrientationsForMonster();

            x = stitchedImage.GetLength(0);
            y = stitchedImage.GetLength(1);

            var nonMonsterHash = 0;

            for (var i = 0; i < x; i++)
            {
                for (var j = 0; j < y; j++)
                {
                    if (stitchedImage[i,
                                      j]
                        == '#')
                    {
                        // All our monster pieces are 'O' now
                        nonMonsterHash++;
                    }
                }
            }

            Console.WriteLine($"Non monster #: {nonMonsterHash}");
        }

        public static T[,] RotateMatrixCounterClockwise<T>(T[,] oldMatrix)
        {
            var newMatrix = new T[oldMatrix.GetLength(1), oldMatrix.GetLength(0)];

            var newRow = 0;

            for (var oldColumn = oldMatrix.GetLength(1) - 1; oldColumn >= 0; oldColumn--)
            {
                var newColumn = 0;
                for (var oldRow = 0; oldRow < oldMatrix.GetLength(0); oldRow++)
                {
                    newMatrix[newRow,
                              newColumn] = oldMatrix[oldRow,
                                                     oldColumn];
                    newColumn++;
                }

                newRow++;
            }

            return newMatrix;
        }

        private static Tile[,] BuildTileImage(List<Tile> tiles,
                                              out int size)
        {
            size = (int)Math.Sqrt(tiles.Count);

            var image = new Tile[size, size];

            var remainingTiles = tiles.ToList();

            Tile FindTile(string top,
                          string left)
            {
                bool IsMatch(Tile tile)
                {
                    bool topSideMatches;

                    if (string.IsNullOrEmpty(top))
                    {
                        topSideMatches = !remainingTiles.Any(t => t.Id != tile.Id && t.Edges.Any(e => e.EqOrRevEq(tile.Top)));
                    }
                    else
                    {
                        topSideMatches = tile.Top == top;
                    }

                    bool leftSideMatches;

                    if (string.IsNullOrEmpty(left))
                    {
                        leftSideMatches = !remainingTiles.Any(t => t.Id != tile.Id && t.Edges.Any(e => e.EqOrRevEq(tile.Left)));
                    }
                    else
                    {
                        leftSideMatches = tile.Left == left;
                    }

                    return topSideMatches && leftSideMatches;
                }

                foreach (var tile in remainingTiles)
                {
                    for (var i = 0; i < 4; i++)
                    {
                        if (IsMatch(tile: tile))
                        {
                            return tile;
                        }

                        tile.RotateRight90();
                    }

                    tile.FlipVertical();

                    for (var i = 0; i < 4; i++)
                    {
                        if (IsMatch(tile: tile))
                        {
                            return tile;
                        }

                        tile.RotateRight90();
                    }
                }

                throw new InvalidOperationException($"Did not find a tile for {top} {left}.");
            }

            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    var top = i == 0
                                  ? null
                                  : image[i - 1,
                                          j]
                                      .Bottom;

                    var left = j == 0
                                   ? null
                                   : image[i,
                                           j - 1]
                                       .Right;

                    var match = FindTile(top,
                                         left);

                    remainingTiles.Remove(match);

                    image[i,
                          j] = match;
                }
            }

            // So far we've only updated the correct edge for the tiles
            // We need to now fix their internal image to match the new orientation
            foreach (var tile in tiles)
            {
                tile.FixOrientation();
            }

            return image;
        }

        private static void DrawImage(char[,] image)
        {
            Console.WriteLine(string.Empty);
            var x = image.GetLength(0);
            var y = image.GetLength(1);

            for (var i = 0; i < x; i++)
            {
                var row = string.Empty;

                for (var j = 0; j < y; j++)
                {
                    row += $"{image[i, j]}";
                }

                Console.WriteLine(row);
            }

            Console.WriteLine(string.Empty);
        }

        private static void DrawTileImage(Tile[,] image)
        {
            Console.WriteLine(string.Empty);
            var x = image.GetLength(0);
            var y = image.GetLength(1);

            for (var i = 0; i < x; i++)
            {
                var row = string.Empty;

                for (var j = 0; j < y; j++)
                {
                    row += $"{image[i, j]?.Id.ToString() ?? "NULL"}  ";
                }

                Console.WriteLine(row);
            }

            Console.WriteLine(string.Empty);
        }

        private static char[,] GetStitchedImage(int stitchedImageSize,
                                                Tile[,] image,
                                                int singleImageSize)
        {
            var stitchedImage = new char[stitchedImageSize, stitchedImageSize];

            var x = image.GetLength(0);
            var y = image.GetLength(1);

            for (var i = 0; i < x; i++)
            {
                for (var j = 0; j < y; j++)
                {
                    var imageAtThisLoc = image[i,
                                               j]
                        .Image;

                    // 8x8 in the example
                    for (var k = 0; k < imageAtThisLoc.Count - 2; k++)
                    {
                        var imageLine = imageAtThisLoc[k + 1];

                        for (var l = 0; l < imageLine.Length - 2; l++)
                        {
                            var xCo = i * singleImageSize + l;
                            var yCo = j * singleImageSize + k;

                            stitchedImage[xCo,
                                          yCo] = imageLine[l + 1];
                        }
                    }
                }
            }

            return stitchedImage;
        }

        private static List<Tile> ParseTiles(string[] fileLines)
        {
            var tiles = new List<Tile>();

            var gettingId = true;

            var newTileId = 0L;
            var newTileImage = new List<string>();

            foreach (var line in fileLines)
            {
                // First line is Id
                if (gettingId)
                {
                    newTileId = long.Parse(line.Replace("Tile ",
                                                        string.Empty)
                                               .TrimEnd(':'));
                    gettingId = false;
                }
                else
                {
                    // Getting an image line
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        newTileImage.Add(line);
                    }
                    else
                    {
                        gettingId = true;
                        tiles.Add(new Tile(newTileId,
                                           newTileImage));
                        newTileImage = new List<string>();
                    }
                }
            }

            return tiles;
        }

        #endregion
    }

    [DebuggerDisplay("{" + nameof(Id) + "}")]
    public class Tile
    {
        #region Constructors

        public Tile(long id,
                    List<string> image)
        {
            Id = id;
            Image = image;

            var edges = new List<string>
                        {
                            Image.First(),
                            Image.Last()
                        };

            var leftEdge = string.Empty;
            var rightEdge = string.Empty;

            foreach (var imageLine in image)
            {
                leftEdge += imageLine[0];
                rightEdge += imageLine[^1];
            }

            edges.Add(leftEdge);
            edges.Add(rightEdge);

            Edges = edges;

            Left = leftEdge;
            Right = rightEdge;
            Top = Image.First();
            Bottom = Image.Last();
        }

        #endregion

        #region Instance Properties

        public string Bottom { get; set; }

        public List<string> Edges { get; set; }

        public long Id { get; }

        public List<string> Image { get; set; }

        public string Left { get; set; }

        public string Right { get; set; }

        public string Top { get; set; }

        #endregion

        #region Instance Methods

        public void FixOrientation()
        {
            bool IsCorrect()
            {
                var leftEdge = string.Empty;
                var rightEdge = string.Empty;

                foreach (var imageLine in Image)
                {
                    leftEdge += imageLine[0];
                    rightEdge += imageLine[^1];
                }

                return Top == Image.First() && Bottom == Image.Last() && Left == leftEdge && Right == rightEdge;
            }

            while (!IsCorrect())
            {
                if (!Top.EqOrRevEq(Image.First())
                    && !Top.EqOrRevEq(Image.Last()))
                {
                    var newImage = Enumerable.Range(0,
                                                    Image.Count)
                                             .Select(x => string.Empty)
                                             .ToList();

                    // Need to transpose image
                    foreach (var imageLine in Image)
                    {
                        for (var i = 0; i < imageLine.Length; i++)
                        {
                            newImage[i] += imageLine[i];
                        }
                    }

                    if (Top.EqOrRevEq(newImage.Last()))
                    {
                        newImage.Reverse();
                    }

                    if (Top.RevString() == newImage.First())
                    {
                        newImage = newImage.Select(x => x.RevString())
                                           .ToList();
                    }

                    Image = newImage;
                }
                else
                {
                    if (Top.EqOrRevEq(Image.Last()))
                    {
                        Image.Reverse();
                    }

                    if (Top.RevString() == Image.First())
                    {
                        Image = Image.Select(x => x.RevString())
                                     .ToList();
                    }

                    if (!IsCorrect())
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }

        public void FlipVertical()
        {
            var tempTop = Top;

            Top = Bottom;

            Bottom = tempTop;

            Left = Left.RevString();

            Right = Right.RevString();
        }

        public void RotateRight90()
        {
            var tempTop = Top;

            Top = Left.RevString();

            Left = Bottom;

            Bottom = Right.RevString();

            Right = tempTop;

            Edges = new List<string>
                    {
                        Top,
                        Left,
                        Bottom,
                        Right
                    };
        }

        #endregion
    }

    public static class Ex
    {
        #region Class Methods

        public static bool EqOrRevEq(this string input,
                                     string other)
        {
            return input.Equals(other) || input.Equals(other.RevString());
        }

        public static string RevString(this string input)
        {
            return new string(input.Reverse()
                                   .ToArray());
        }

        #endregion
    }
}