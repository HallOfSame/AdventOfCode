using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Day06
{
    public static class Day6Beautiful
    {
        #region Class Methods

        public static async Task<(int partOne, int partTwo)> GetAnswers()
        {
            return ((await File.ReadAllTextAsync("PuzzleInput.txt")).Split($"{Environment.NewLine}{Environment.NewLine}")
                                                                    .AsParallel()
                                                                    .Select(g => g.Split(Environment.NewLine))
                                                                    .Select(x => x.Aggregate(Enumerable.Empty<char>(),
                                                                                             (l,
                                                                                              y) => l.Union(y))
                                                                                  .Count())
                                                                    .Sum(), (await File.ReadAllTextAsync("PuzzleInput.txt")).Split($"{Environment.NewLine}{Environment.NewLine}")
                                                                                                                            .AsParallel()
                                                                                                                            .Select(g => g.Split(Environment.NewLine))
                                                                                                                            .Select(x => x.Aggregate(x[0] as IEnumerable<char>,
                                                                                                                                                     (l,
                                                                                                                                                      y) => y.Any()
                                                                                                                                                                ? l.Intersect(y)
                                                                                                                                                                : l)
                                                                                                                                          .Count())
                                                                                                                            .Sum());
        }

        #endregion
    }
}