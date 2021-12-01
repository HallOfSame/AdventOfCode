using System;
using System.Collections.Generic;
using System.Linq;

namespace Day09
{
    public class XmasCipherProcessor
    {
        #region Fields

        private readonly int preambleLength;

        #endregion

        #region Constructors

        public XmasCipherProcessor(int preambleLength)
        {
            this.preambleLength = preambleLength;
        }

        #endregion

        #region Instance Methods

        public long FindInvalidValue(List<long> input)
        {
            for (var i = preambleLength; i < input.Count; i++)
            {
                var sumOptions = input.GetRange(i - preambleLength,
                                                preambleLength);

                var currentValue = input[i];

                var validSum = sumOptions.Any(x => sumOptions.Any(y => x + y == currentValue && x != y));

                if (!validSum)
                {
                    return currentValue;
                }
            }

            throw new InvalidOperationException("No invalid value found.");
        }

        public long GetEncryptionWeakness(List<long> input)
        {
            var targetValue = FindInvalidValue(input);

            // The values past the target will be too large
            var inputToCheck = input.GetRange(0,
                                              input.IndexOf(targetValue));

            List<long> contiguousRange = null;

            for (var i = 0; i < inputToCheck.Count; i++)
            {
                var testList = inputToCheck.Skip(i);

                var sum = 0L;

                var currentIndex = 0;

                contiguousRange = testList.TakeWhile((x,
                                                      idx) =>
                                                     {
                                                         sum += x;
                                                         currentIndex = idx;

                                                         return sum < targetValue;
                                                     })
                                          .ToList();

                if (sum == targetValue)
                {
                    // Using sum <= targetValue fails to find the list
                    // But because of how TakeWhile works, the last value isn't added to the range
                    contiguousRange.Add(testList.ElementAt(currentIndex));
                    break;
                }
                else
                {
                    contiguousRange = null;
                }
            }

            if (contiguousRange == null)
            {
                throw new InvalidOperationException($"No range found that sums to {targetValue}.");
            }

            return contiguousRange.Min() + contiguousRange.Max();
        }

        #endregion
    }
}