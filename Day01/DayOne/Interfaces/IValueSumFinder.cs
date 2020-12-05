using System.Collections.Generic;
using System.Threading.Tasks;

namespace DayOne.Interfaces
{
    public interface IValueSumFinder
    {
        #region Instance Methods

        /// <summary>
        /// Finds the two values from <paramref name="values" /> that add to <paramref name="requiredSum" />.
        /// </summary>
        /// <param name="values">
        /// The list of potential values to use.
        /// </param>
        /// <param name="numberOfValues">
        /// The number of values to sum to reach <paramref name="requiredSum" />.
        /// </param>
        /// <param name="requiredSum">
        /// The sum the return values should sum to.
        /// </param>
        /// <returns>
        /// The two values that add to <paramref name="requiredSum" /> from <paramref name="values" />.
        /// </returns>
        Task<List<int>> GetValuesForSumAsync(List<int> values,
                                             int numberOfValues,
                                             int requiredSum);

        #endregion
    }
}