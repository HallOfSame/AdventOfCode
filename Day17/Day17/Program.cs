using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Day17
{
    internal class Program
    {
        #region Class Methods

        private static void DrawZLevel(List<List<Cube>> zLevel)
        {
            var builder = new StringBuilder();

            builder.Append(Environment.NewLine);

            foreach (var row in zLevel)
            {
                foreach (var cube in row)
                {
                    builder.Append(cube.State
                                       ? '#'
                                       : '.');
                }

                builder.Append(Environment.NewLine);
            }

            Console.WriteLine(builder.ToString());
        }

        private static void Main(string[] args)
        {
            //var fileText = File.ReadAllText("PuzzleInput.txt");

            //for (var i = 1; i <= 7; i++)
            //{
            //    Console.WriteLine($"{i} - {Other.Puzzle1(fileText, i)}");
            //}

            var simTurns = 6;

            // Add padding around the edges
            var padding = simTurns + 2;

            var fileLines = File.ReadAllLines("PuzzleInput.txt");

            // First build z Level 0
            var cubeRows = new List<List<Cube>>();

            foreach (var line in fileLines)
            {
                var row = new List<Cube>();

                // First add the input
                row.AddRange(line.ToCharArray()
                                 .Select(x => new Cube(x == '#')));

                // Then pad the front and back
                var fullRow = Enumerable.Range(0,
                                               padding)
                                        .Select(x => new Cube(false))
                                        .Concat(row)
                                        .Concat(Enumerable.Range(0,
                                                                 padding)
                                                          .Select(x => new Cube(false)))
                                        .ToList();

                cubeRows.Add(fullRow);
            }

            // Now we have enough rows for the initial input that are wide enough to not expand
            // Need to add more inactive rows that aren't show in the input
            var width = cubeRows[0]
                .Count;

            for (var i = 0; i < padding; i++)
            {
                cubeRows.Insert(i,
                                Enumerable.Range(0,
                                                 width)
                                          .Select(x => new Cube(false))
                                          .ToList());
            }

            for (var i = 0; i < padding; i++)
            {
                cubeRows.Add(Enumerable.Range(0,
                                              width)
                                       .Select(x => new Cube(false))
                                       .ToList());
            }

            var height = cubeRows.Count;

            // Now we need to create the empty z levels above and below
            var zLevels = new Dictionary<int, List<List<Cube>>>();

            zLevels[0] = cubeRows;

            // Negative levels
            for (var i = 0 - padding; i < 0; i++)
            {
                zLevels[i] = Enumerable.Range(0,
                                              height)
                                       .Select(r => Enumerable.Range(0,
                                                                     width)
                                                              .Select(c => new Cube(false))
                                                              .ToList())
                                       .ToList();
            }

            // Positive levels
            for (var i = 1; i < padding; i++)
            {
                zLevels[i] = Enumerable.Range(0,
                                              height)
                                       .Select(r => Enumerable.Range(0,
                                                                     width)
                                                              .Select(c => new Cube(false))
                                                              .ToList())
                                       .ToList();
            }

            void GetNeighborsForLevel(int x,
                                      int y,
                                      bool includeXY,
                                      List<Cube> neighborList,
                                      List<List<Cube>> zLevel)
            {
                for (var i = x - 1; i <= x + 1; i++)
                {
                    if (i < 0
                        || i >= height)
                    {
                        continue;
                    }

                    for (var k = y - 1; k <= y + 1; k++)
                    {
                        if (k < 0
                            || k >= width)
                        {
                            continue;
                        }

                        if (i == x
                            && k == y
                            && !includeXY)
                        {
                            // This is here to prevent adding the cube itself as a neighbor
                            continue;
                        }

                        neighborList.Add(zLevel[i][k]);
                    }
                }
            }

            List<Cube> GetNeighbors(int z,
                                    int x,
                                    int y)
            {
                var neighborList = new List<Cube>(26);

                // Get neighbors from Z above
                if (zLevels.TryGetValue(z + 1,
                                        out var levelAbove))
                {
                    GetNeighborsForLevel(x,
                                         y,
                                         true,
                                         neighborList,
                                         levelAbove);
                }

                // Get neighbors from Z below
                if (zLevels.TryGetValue(z - 1,
                                        out var levelBelow))
                {
                    GetNeighborsForLevel(x,
                                         y,
                                         true,
                                         neighborList,
                                         levelBelow);
                }

                // Get neighbors on same Z
                GetNeighborsForLevel(x,
                                     y,
                                     false,
                                     neighborList,
                                     zLevels[z]);

                Debug.Assert(neighborList.Count <= 26);
                Debug.Assert(neighborList.Any());

                return neighborList;
            }

            var minZ = zLevels.Keys.Min();
            var maxZ = zLevels.Keys.Max();

            // Now need to iterate through and hook up cube neighbors
            for (var zIdx = minZ; zIdx < maxZ; zIdx++)
            {
                var cubesAtThisLevel = zLevels[zIdx];

                // Row
                for (var rowIdx = 0; rowIdx < cubesAtThisLevel.Count; rowIdx++)
                {
                    var row = cubesAtThisLevel[rowIdx];

                    // Column
                    for (var colIdx = 0; colIdx < row.Count; colIdx++)
                    {
                        var currentCube = row[colIdx];

                        currentCube.Neighbors.AddRange(GetNeighbors(zIdx,
                                                                    rowIdx,
                                                                    colIdx));
                    }
                }
            }

            var dimension = new Dimension(zLevels);

            var simulator = new Simulator();

            var stopWatch = new Stopwatch();

            for (var i = 0; i < simTurns; i++)
            {
                stopWatch.Restart();
                dimension = simulator.RunOneTurn(dimension);
                stopWatch.Stop();
                Console.WriteLine($"Turn {i}. Time {stopWatch.ElapsedMilliseconds}ms. Cubes {dimension.ActiveCubeCount}.");
            }

            var activeCubes = dimension.AllCubes.Count(x => x.State);

            Console.WriteLine($"Number of active cubes after {simTurns} turns. {activeCubes}.");
        }

        #endregion
    }

    public class Simulator
    {
        #region Instance Methods

        public Dimension RunOneTurn(Dimension input)
        {
            // Find the next state for each cube
            foreach (var level in input.ZLevels)
            {
                foreach (var row in level.Value)
                {
                    foreach (var column in row)
                    {
                        bool newState;

                        var activeNeighbors = column.Neighbors.Count(x => x.State);

                        if (column.State)
                        {
                            if (activeNeighbors == 2
                                || activeNeighbors == 3)
                            {
                                newState = true;
                            }
                            else
                            {
                                newState = false;
                            }
                        }
                        else
                        {
                            if (activeNeighbors == 3)
                            {
                                newState = true;
                            }
                            else
                            {
                                newState = false;
                            }
                        }

                        column.NextState = newState;
                    }
                }
            }

            // Process changes
            input.AllCubes.ForEach(c => c.State = c.NextState);

            return input;
        }

        #endregion
    }

    public class Dimension
    {
        #region Constructors

        public Dimension(Dictionary<int, List<List<Cube>>> zLevels)
        {
            ZLevels = zLevels;
        }

        #endregion

        #region Instance Properties

        public int ActiveCubeCount
        {
            get
            {
                return AllCubes.Count(x => x.State);
            }
        }

        public List<Cube> AllCubes
        {
            get
            {
                return ZLevels.SelectMany(z => z.Value.SelectMany(r => r))
                              .ToList();
            }
        }

        public Dictionary<int, List<List<Cube>>> ZLevels { get; }

        #endregion
    }

    [DebuggerDisplay("{State ? '#' : '.'}")]
    public class Cube
    {
        #region Constructors

        public Cube(bool initState)
        {
            State = initState;
            Neighbors = new List<Cube>(26);
        }

        #endregion

        #region Instance Properties

        public List<Cube> Neighbors { get; }

        public bool NextState { get; set; }

        public bool State { get; set; }

        #endregion
    }
}