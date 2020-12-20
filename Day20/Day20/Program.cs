using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day20
{
    public class Program
    {
        static void Main(string[] args)
        {
            var fileLines = File.ReadAllLines("PuzzleInput.txt");

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

            // Corners are ones w/ 3 matches
            var corners = matchingTiles.Where(x => x.Value.Count == 2)
                                       .ToList();

            Debug.Assert(corners.Count == 4);

            Console.WriteLine($"Corner IDs multiplied: {corners.Select(x => x.Key).Aggregate(1L, (x, y) => x * y)}");
        }
    }

    public class Tile
    {
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
        }

        public long Id { get; }

        public List<string> Image { get; }

        public List<string> Edges { get; }

        public string Left { get; set; }

        public string Right { get; set; }

        public string Top { get; set; }

        public string Bottom { get; set; }
    }
}
