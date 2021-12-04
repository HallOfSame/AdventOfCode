using System;
using System.Linq;

namespace Day04
{
    internal class Program
    {
        #region Class Methods

        private static void Main(string[] args)
        {
            var min = 0;
            var max = 1;

            var range = Enumerable.Range(min,
                                         max - min + 1)
                                  .ToList();

            // PT 1
            var validPassword = 0;

            for (var i = 0; i < range.Count; i++)
            {
                var testValue = range[i]
                                .ToString()
                                .ToCharArray();

                var isValid = testValue.Where((c,
                                               idx) => idx < testValue.Length - 1 && testValue[idx + 1] == c)
                                       .Any();

                if (isValid)
                {
                    isValid = testValue.SequenceEqual(testValue.OrderBy(x => x));
                }

                if (isValid)
                {
                    validPassword++;
                }
            }

            Console.WriteLine($"Valid options: {validPassword}.");

            // PT 2
            validPassword = 0;

            for (var i = 0; i < range.Count; i++)
            {
                var testValue = range[i]
                                .ToString()
                                .ToCharArray();

                // Check the third value isn't the same as the two before
                var isValid = testValue.Where((c,
                                               idx) =>
                                              {
                                                  if (idx < testValue.Length - 2)
                                                  {
                                                      return testValue[idx + 1] == c && testValue[idx + 2] != c && (idx <= 0 || testValue[idx - 1] != c);
                                                  }

                                                  if (idx < testValue.Length - 1)
                                                  {
                                                      return testValue[idx + 1] == c && (idx <= 0 || testValue[idx - 1] != c);
                                                  }

                                                  return false;
                                              })
                                       .Any();

                if (isValid)
                {
                    isValid = testValue.SequenceEqual(testValue.OrderBy(x => x));
                }

                if (isValid)
                {
                    validPassword++;
                }
            }

            Console.WriteLine($"Valid options PT2: {validPassword}.");
        }

        #endregion
    }
}