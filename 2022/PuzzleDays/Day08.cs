using Helpers.Extensions;
using Helpers.FileReaders;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day08 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var visibleTrees = 0;

            for (var x = 0; x < grid.GetHeight(); x++)
            {
                for (var y = 0; y < grid.GetWidth(); y++)
                {
                    if (IsVisible(x,
                                  y))
                    {
                        // Console.WriteLine($"({x}, {y}) is visible.");
                        visibleTrees += 1;
                    }
                }
            }

            return visibleTrees.ToString();
        }

        private bool IsVisible(int x,
                               int y)
        {
            return IsVisibleEast(x,
                                 y)
                   || IsVisibleWest(x,
                                    y)
                   || IsVisibleNorth(x,
                                     y)
                   || IsVisibleSouth(x,
                                     y);
        }

        private bool IsVisibleNorth(int x,
                                    int y)
        {
            var height = GetTreeHeight(x, y);

            for (var testX = x - 1; testX >= 0; testX--)
            {
                var testHeight = GetTreeHeight(testX,
                                               y);

                if (testHeight >= height)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsVisibleSouth(int x,
                                    int y)
        {
            var height = GetTreeHeight(x, y);

            for (var testX = x + 1; testX < grid.GetHeight(); testX++)
            {
                var testHeight = GetTreeHeight(testX,
                                               y);

                if (testHeight >= height)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsVisibleEast(int x,
                                        int y)
        {
            var height = GetTreeHeight(x, y);

            for (var testY = y - 1; testY >= 0; testY--)
            {
                var testHeight = GetTreeHeight(x,
                                               testY);

                if (testHeight >= height)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsVisibleWest(int x,
                                   int y)
        {
            var height = GetTreeHeight(x,
                                       y);

            for (var testY = y + 1; testY < grid.GetWidth(); testY++)
            {
                var testHeight = GetTreeHeight(x,
                                               testY);

                if (testHeight >= height)
                {
                    return false;
                }
            }

            return true;
        }

        private int GetTreeHeight(int x,
                                  int y)
        {
            return grid.GetCoord(x,
                                 y);
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            var bestScenicScore = 0;

            for (var x = 0; x < grid.GetHeight(); x++)
            {
                for (var y = 0; y < grid.GetWidth(); y++)
                {
                    bestScenicScore = Math.Max(bestScenicScore,
                                               GetScenicScore(x,
                                                              y));
                }
            }

            return bestScenicScore.ToString();
        }

        private int GetScenicScore(int x,
                                   int y)
        {
            return TreesVisibleNorth(x,
                                     y)
                   * TreesVisibleSouth(x,
                                       y)
                   * TreesVisibleWest(x,
                                      y)
                   * TreesVisibleEast(x,
                                      y);
        }

        private int TreesVisibleNorth(int x,
                                      int y)
        {
            var height = GetTreeHeight(x,
                                       y);

            var treesSeen = 0;

            for (var testX = x - 1; testX >= 0; testX--)
            {
                var testHeight = GetTreeHeight(testX,
                                               y);

                treesSeen += 1;

                if (testHeight >= height)
                {
                    break;
                }
            }

            return treesSeen;
        }

        private int TreesVisibleEast(int x,
                                     int y)
        {
            var height = GetTreeHeight(x,
                                       y);

            var treesSeen = 0;

            for (var testY = y - 1; testY >= 0; testY--)
            {
                var testHeight = GetTreeHeight(x,
                                               testY);

                treesSeen += 1;

                if (testHeight >= height)
                {
                    break;
                }
            }

            return treesSeen;
        }

        private int TreesVisibleSouth(int x,
                                      int y)
        {
            var height = GetTreeHeight(x,
                                       y);

            var treesSeen = 0;

            for (var testX = x + 1; testX < grid.GetHeight(); testX++)
            {
                var testHeight = GetTreeHeight(testX,
                                               y);

                treesSeen++;

                if (testHeight >= height)
                {
                    break;
                }
            }

            return treesSeen;
        }

        private int TreesVisibleWest(int x,
                                      int y)
        {
            var height = GetTreeHeight(x,
                                       y);

            var treesSeen = 0;

            for (var testY = y + 1; testY < grid.GetWidth(); testY++)
            {
                var testHeight = GetTreeHeight(x,
                                               testY);

                treesSeen++;

                if (testHeight >= height)
                {
                    break;
                }
            }

            return treesSeen;
        }

        public override async Task ReadInput()
        {
            var arrayList = await new _2DArrayReader().ReadInputFromFile();

            grid = arrayList.To2DArray();
        }

        int[,] grid;
    }
}