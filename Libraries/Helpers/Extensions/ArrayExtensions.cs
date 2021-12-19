using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers.Extensions
{
    public static class ArrayExtensions
    {
        #region Class Methods

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements,
                                                                  int length)
        {
            return length == 0
                       ? new[]
                         {
                             Array.Empty<T>()
                         }
                       : elements.SelectMany((e,
                                              i) => elements.Skip(i + 1)
                                                            .Combinations(length - 1)
                                                            .Select(c => new[]
                                                                         {
                                                                             e
                                                                         }.Concat(c)));
        }

        #endregion
    }
}