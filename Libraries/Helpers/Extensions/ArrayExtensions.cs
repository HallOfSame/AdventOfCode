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

        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> list,
                                                                  int length)
        {
            if (length == 1)
                return list.Select(t => new T[]
                                        {
                                            t
                                        });

            return Permutations(list,
                                length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                            (t1,
                             t2) => t1.Concat(new[]
                                              {
                                                  t2
                                              }));
        }

        public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null)
            {
                yield break;
            }

            var list = sequence.ToList();

            if (!list.Any())
            {
                yield return Enumerable.Empty<T>();
            }
            else
            {
                var startingElementIndex = 0;

                foreach (var startingElement in list)
                {
                    var index = startingElementIndex;
                    var remainingItems = list.Where((e, i) => i != index);

                    foreach (var permutationOfRemainder in remainingItems.Permute())
                    {
                        yield return permutationOfRemainder.Prepend(startingElement);
                    }

                    startingElementIndex++;
                }
            }
        }

        #endregion
    }
}