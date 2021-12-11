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
                                   Func<T, string> getDisplayText)
        {
            var borderLength = array.GetLength(0);

            var stringBuilder = new StringBuilder();

            void DrawBorder()
            {
                stringBuilder.AppendLine(new string(Enumerable.Repeat('-',
                                                                      borderLength)
                                                              .ToArray()));
            }

            DrawBorder();

            for (var y = 0; y < array.GetLength(1); y++)
            {
                for (var x = 0; x < borderLength; x++)
                {
                    stringBuilder.Append(getDisplayText(array[x,
                                                              y]));
                }

                stringBuilder.Append(Environment.NewLine);
            }

            DrawBorder();

            AnsiConsole.WriteLine(stringBuilder.ToString());
        }

        public static bool IsValidCoordinate<T>(this T[,] array,
                                                Coordinate coordinate)
        {
            return array.IsValidCoordinate(coordinate.X,
                                           coordinate.Y);
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

        #endregion
    }
}