using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.FileReaders;
using Helpers.Structure;

namespace PuzzleDays
{
    public class Day13 : ProblemBase
    {
        protected override async Task<string> SolvePartOneInternal()
        {
            var result = rowSets.Select((rs, idx) =>
                {
                    var horizLine = GetReflectionLine(rs);

                    if (horizLine is not null)
                    {
                        return horizLine * 100;
                    }

                    var vertLine = GetReflectionLine(columnSets[idx]);

                    if (vertLine is null)
                    {
                        throw new Exception($"Did not find a reflection line for index {idx}");
                    }

                    return vertLine;

                })
                .Sum();

            return result.ToString();
        }

        private int? GetReflectionLine(List<string> rowsOrColumns)
        {
            for (var possibleFirstIndexAfterReflectLine = 1; possibleFirstIndexAfterReflectLine < rowsOrColumns.Count; possibleFirstIndexAfterReflectLine++)
            {
                // Total rows or columns == 10
                // If first row/col after == idx 4
                // Then we either check 0,1,2,3 or 4,5,6,7,8,9,10 
                // Since it's uneven, check 0-3 against 4-7
                var rowsToCheck = Math.Min(possibleFirstIndexAfterReflectLine,
                                           rowsOrColumns.Count - possibleFirstIndexAfterReflectLine);

                var isValid = true;

                // Keeping w/ above example, we have 4 row/col to check
                // Starting with 3 & 4
                // Work outwards, 2&5, 1&6, ....
                for (var i = 0; i < rowsToCheck; i++)
                {
                    var bottomRowIndex = possibleFirstIndexAfterReflectLine + i;
                    var topRowIndex = (possibleFirstIndexAfterReflectLine - 1) - i;

                    if (!string.Equals(rowsOrColumns[bottomRowIndex], rowsOrColumns[topRowIndex]))
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                {
                    // This ends up also being the count of lines above / to the left
                    return possibleFirstIndexAfterReflectLine;
                }
            }

            return null;
        }

        protected override async Task<string> SolvePartTwoInternal()
        {
            throw new NotImplementedException();
        }

        private List<List<string>> rowSets { get; set; }

        private List<List<string>> columnSets { get; set; }

        public override async Task ReadInput()
        {
            var allStrings = await new StringFileReader().ReadInputFromFile();

            rowSets = new List<List<string>>();

            var currentRowSet = new List<string>();

            foreach (var line in allStrings)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    rowSets.Add(currentRowSet.ToList());
                    currentRowSet.Clear();
                    continue;
                }

                currentRowSet.Add(line);
            }

            rowSets.Add(currentRowSet);

            columnSets = new List<List<string>>();

            foreach (var rowSet in rowSets)
            {
                var width = rowSet[0]
                    .Length;

                var columns = new List<string>();

                for (var x = 0; x < width; x++)
                {
                    var sb = new StringBuilder();

                    for (var y = 0; y < rowSet.Count; y++)
                    {
                        sb.Append(rowSet[y][x]);
                    }

                    columns.Add(sb.ToString());
                }

                columnSets.Add(columns);
            }
        }
    }
}
