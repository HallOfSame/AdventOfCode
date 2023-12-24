using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Helpers.Maps;

using Spectre.Console;

namespace Helpers.Extensions
{
    public static class _2DArrayExtensions
    {
        #region Class Methods

        public static void Draw<T>(this T[,] array,
                                   Func<T, string> getDisplayText,
                                   string title = null)
        {
            var borderLength = array.GetLength(0);

            var stringBuilder = new StringBuilder();

            void DrawBorder()
            {
                stringBuilder.AppendLine(new string(Enumerable.Repeat('-',
                                                                      borderLength)
                                                              .ToArray()));
            }

            //DrawBorder();

            for (var y = 0; y < array.GetLength(1); y++)
            {
                for (var x = 0; x < borderLength; x++)
                {
                    stringBuilder.Append(getDisplayText(array[x,
                                                              y]));
                }

                stringBuilder.Append(Environment.NewLine);
            }

            //DrawBorder();

            var panel = new Panel(stringBuilder.ToString())
                        {
                            Header = new PanelHeader(title ?? "2D Array")
                        };

            AnsiConsole.Write(panel);
        }

        public static bool IsValidCoordinate<T>(this T[,] array,
                                                Coordinate coordinate)
        {
            return array.IsValidCoordinate((int)coordinate.X,
                                           (int)coordinate.Y);
        }

        public static bool IsValidCoordinate<T>(this T[,] array,
                                                int x,
                                                int y)
        {
            if (x < 0)
            {
                return false;
            }

            if (y < 0)
            {
                return false;
            }

            var xLength = array.GetLength(0);

            if (x >= xLength)
            {
                return false;
            }

            var yLength = array.GetLength(1);

            if (y >= yLength)
            {
                return false;
            }

            return true;
        }

        public static T[,] To2DArray<T>(this List<T[]> inputList)
        {
            var xLength = inputList[0]
                .Length;

            var yLength = inputList.Count;

            var output = new T[xLength, yLength];

            for (var y = 0; y < yLength; y++)
            {
                var currentRow = inputList[y];

                for (var x = 0; x < xLength; x++)
                {
                    output[x,
                           y] = currentRow[x];
                }
            }

            return output;
        }

        /// <summary>
        /// This is the max Y value for coordinates
        /// </summary>
        public static int GetWidth<T>(this T[,] array)
        {
            return array.GetLength(0);
        }

        /// <summary>
        /// This is the max X value for coordinates
        /// </summary>
        public static int GetHeight<T>(this T[,] array)
        {
            return array.GetLength(1);
        }

        /// <summary>
        /// This is always backwards from how it seems like it should work.
        /// </summary>
        /// <param name="x">X coord of the grid to get.</param>
        /// <param name="y">Y coord of the grid to get.</param>
        public static T GetCoord<T>(this T[,] array,
                                    int x,
                                    int y)
        {
            // Somehow you index with y first when thinking of them like coordinates
            return array[y,
                         x];
        }

        #endregion
    }
}